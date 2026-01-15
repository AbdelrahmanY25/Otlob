using Otlob.Core.Entities;

namespace Otlob.Services;

public class OrderService(IUnitOfWorkRepository unitOfWorkRepository,
                          ICartService cartService,
                          ITempOrderService tempOrderService,
                          IHttpContextAccessor httpContextAccessor,
                          IAdminDailyAnalyticsService adminDailyAnalyticsService,
                          IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
                          IPromoCodeService promoCodeService,
                          IMealsAnalyticsService mealsAnalyticsService) : IOrderService
{
    private readonly ICartService _cartService = cartService;
    private readonly ITempOrderService _tempOrderService = tempOrderService;
    private readonly IPromoCodeService _promoCodeService = promoCodeService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IMealsAnalyticsService _mealsAnalyticsService = mealsAnalyticsService;
    private readonly IAdminDailyAnalyticsService _adminDailyAnalyticsService = adminDailyAnalyticsService;
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;

    public async Task<Result> PlaceOrder(PaymentMethod paymentMethod, string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0)
    {        
        var prepareOrderDataResult = await PrepareOrderData(paymentMethod, specialNotes, promoCodeId, discountAmount);

        if (prepareOrderDataResult.IsFailure)
            return Result.Failure(prepareOrderDataResult.Error);

        return Result.Success();
    }

    public Result<string> CreateStripeSession(string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        // Get cart data with meal names for Stripe display
        var cartData = _unitOfWorkRepository.Carts
            .GetOneWithSelect(
                expression: c => c.UserId == userId,
                tracked: false,
                selector: c => new
                {
                    c.Id,
                    c.RestaurantId,
                    CartDetails = c.CartDetails.Select(cd => new
                    {
                        cd.MealId,
                        MealName = cd.Meal.Name,
                        cd.MealDetails,
                        cd.Quantity,
                        cd.MealPrice,
                        cd.ItemsPrice,
                        cd.AddOnsPrice,
                        cd.TotalPrice
                    }).ToList()
                }
            );

        if (cartData is null || cartData.CartDetails.Count == 0)
            return Result.Failure<string>(CartErrors.CartIsEmpty);

        // Get restaurant delivery fee
        var restaurantData = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == cartData.RestaurantId,
                tracked: false,
                selector: r => new { r.DeliveryFee }
            );

        if (restaurantData is null)
            return Result.Failure<string>(RestaurantErrors.NotFound);

        // Get user delivery address data for the order
        var userData = _unitOfWorkRepository.Users
            .GetOneWithSelect(
                expression: u => u.Id == userId,
                tracked: false,
                selector: u => new
                {
                    u.PhoneNumber,
                    Address = u.UserAddress
                        .Where(a => a.IsDeliveryAddress)
                        .Select(a => new
                        {
                            a.CustomerAddress,
                            a.Location
                        })
                        .FirstOrDefault()
                }
            );

        if (userData?.Address is null)
            return Result.Failure<string>(AddressErrors.NoDeliveryAddress);

        decimal subTotal = cartData.CartDetails.Sum(cd => cd.TotalPrice);
        decimal serviceFee = subTotal * 0.05m;

        // Create temp order to store data during payment
        Order tempOrder = new()
        {
            UserId = userId,
            RestaurantId = cartData.RestaurantId,
            CustomerPhoneNumber = userData.PhoneNumber!,
            DeliveryAddress = userData.Address.CustomerAddress,
            DeliveryAddressLocation = userData.Address.Location,
            SubPrice = subTotal,
            DeliveryFee = restaurantData.DeliveryFee,
            ServiceFeePrice = serviceFee,
            DiscountAmount = discountAmount,
            PromoCodeId = promoCodeId,
            Notes = specialNotes,
            Method = PaymentMethod.Credit,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            OrderDetails = cartData.CartDetails.Select(cd => new OrderDetails
            {
                MealId = cd.MealId,
                MealDetails = cd.MealDetails,
                MealQuantity = cd.Quantity,
                MealPrice = cd.MealPrice,
                ItemsPrice = cd.ItemsPrice,
                AddOnsPrice = cd.AddOnsPrice
            }).ToList()
        };

        // Store temp order for later retrieval after payment
        TempOrder storedTempOrder = _tempOrderService.AddTempOrder(tempOrder);

        // Build Stripe session
        var request = _httpContextAccessor.HttpContext!.Request;
        string baseUrl = $"{request.Scheme}://{request.Host}";

        SessionCreateOptions options = new()
        {
            PaymentMethodTypes = ["card"],
            LineItems = [],
            Mode = "payment",
            SuccessUrl = $"{baseUrl}/Customer/Order/FinishPaymentProcess?tempOrderId={storedTempOrder.Id}",
            CancelUrl = $"{baseUrl}/Customer/CheckOut/CheckOut",
            Metadata = new Dictionary<string, string>
            {
                { "tempOrderId", storedTempOrder.Id },
                { "userId", userId }
            }
        };

        // Add meal items to Stripe
        foreach (var item in cartData.CartDetails)
        {
            string description = BuildMealDescription(item.MealDetails);
            decimal unitPrice = item.MealPrice + item.ItemsPrice + item.AddOnsPrice;

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "EGP",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.MealName,
                        Description = string.IsNullOrEmpty(description) ? null : description
                    },
                    UnitAmount = (long)Math.Ceiling(unitPrice * 100),
                },
                Quantity = item.Quantity,
            });
        }

        // Add delivery fee
        if (restaurantData.DeliveryFee > 0)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "EGP",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Delivery Fee",
                        Description = "Fee for delivering your order",
                    },
                    UnitAmount = (long)Math.Ceiling(restaurantData.DeliveryFee * 100),
                },
                Quantity = 1,
            });
        }

        // Add service fee
        if (serviceFee > 0)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "EGP",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Service Fee",
                        Description = "Platform and support fee",
                    },
                    UnitAmount = (long)Math.Ceiling(serviceFee * 100),
                },
                Quantity = 1,
            });
        }

        // Apply promo code discount using Stripe Coupon
        if (discountAmount > 0)
        {
            try
            {
                // Create a one-time coupon for this discount
                var couponService = new Stripe.CouponService();
                var coupon = couponService.Create(new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)Math.Ceiling(discountAmount * 100),
                    Currency = "EGP",
                    Duration = "once",
                    Name = promoCodeId.HasValue ? $"Promo Code Discount" : "Discount"
                });

                options.Discounts =
                [
                    new SessionDiscountOptions { Coupon = coupon.Id }
                ];
            }
            catch
            {
                // If coupon creation fails, continue without discount
                // The order will still have the discount recorded for reference
            }
        }

        try
        {
            var service = new SessionService();
            var session = service.Create(options);
            return Result.Success(session.Url);
        }
        catch
        {
            _tempOrderService.RemoveTempOrder(storedTempOrder);
            return Result.Failure<string>(OrderErrors.StripeSessionFailed);
        }
    }

    public async Task<Result> FinishCreditPayment(string tempOrderId)
    {
        TempOrder? tempOrder = _tempOrderService.GetTempOrder(tempOrderId);

        if (tempOrder is null)
            return Result.Failure(OrderErrors.SessionExpired);

        // Deserialize the stored order data
        var orderData = System.Text.Json.JsonSerializer.Deserialize<TempOrderData>(tempOrder.OrderData!,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (orderData is null)
            return Result.Failure(OrderErrors.SessionExpired);

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            // Get current cart to delete after order
            var cartId = _unitOfWorkRepository.Carts
                .GetOneWithSelect(
                    expression: c => c.UserId == orderData.UserId,
                    tracked: false,
                    selector: c => c.Id
                );

            // Create the order from stored data
            Order order = new()
            {
                UserId = orderData.UserId,
                RestaurantId = orderData.RestaurantId,
                CustomerPhoneNumber = orderData.CustomerPhoneNumber,
                DeliveryAddress = orderData.DeliveryAddress,
                DeliveryAddressLocation = new Point(orderData.DeliveryLocationX, orderData.DeliveryLocationY) { SRID = 4326 },
                SubPrice = orderData.SubPrice,
                DeliveryFee = orderData.DeliveryFee,
                ServiceFeePrice = orderData.ServiceFeePrice,
                DiscountAmount = orderData.DiscountAmount,
                PromoCodeId = orderData.PromoCodeId,
                Notes = orderData.Notes,
                Method = orderData.Method,
                OrderDate = orderData.OrderDate,
                Status = orderData.Status,
            };

            // Add the order
            _unitOfWorkRepository.Orders.Add(order);
            _unitOfWorkRepository.SaveChanges();

            // Record promo code usage if applicable
            if (orderData.PromoCodeId.HasValue && orderData.DiscountAmount > 0)
                _promoCodeService.RecordPromoCodeUsage(orderData.PromoCodeId.Value, order.Id, orderData.UserId, orderData.DiscountAmount);

            // Create and add order details
            List<OrderDetails> orderDetailsList = orderData.OrderDetails.Select(m => new OrderDetails
            {
                OrderId = order.Id,
                MealId = m.MealId,
                MealDetails = m.MealDetails,
                MealQuantity = m.MealQuantity,
                MealPrice = m.MealPrice,
                ItemsPrice = m.ItemsPrice,
                AddOnsPrice = m.AddOnsPrice
            }).ToList();

            await _unitOfWorkRepository.OrderDetails.AddRangeAsync(orderDetailsList);
            _unitOfWorkRepository.SaveChanges();

            // Delete cart
            if (cartId > 0)
                _cartService.Delete(cartId);

            // Remove temp order
            _tempOrderService.RemoveTempOrder(tempOrder);

            // update AdminDailyAnalytic
            _adminDailyAnalyticsService.InitialUpdate();

            // update RestaurantDailyAnalytic
            _restaurantDailyAnalyticsService.InitialUpdate(order.RestaurantId);

           // update MealsAnalytics
            foreach (var detail in orderDetailsList)
            {
                decimal saleAmount = detail.MealPrice + detail.ItemsPrice + detail.AddOnsPrice;
                _mealsAnalyticsService.UpdateSales(order.RestaurantId, detail.MealId, detail.MealQuantity, saleAmount);
            }

            transaction.Commit();

            return Result.Success();
        }
        catch
        {
            transaction.Rollback();
            return Result.Failure(OrderErrors.AddOrderField);
        }
    }



    private static string BuildMealDescription(string mealDetailsJson)
    {
        if (string.IsNullOrEmpty(mealDetailsJson))
            return string.Empty;

        try
        {
            var node = JsonNode.Parse(mealDetailsJson);
            var items = node?["items"]?.AsArray()
                .Select(x => x?["Name"]?.GetValue<string>())
                .Where(s => !string.IsNullOrEmpty(s)) ?? [];

            var addOns = node?["addOns"]?.AsArray()
                .Select(x => x?["Name"]?.GetValue<string>())
                .Where(s => !string.IsNullOrEmpty(s)) ?? [];

            var combined = items.Concat(addOns!);
            return string.Join(", ", combined);
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<Result> PrepareOrderData(PaymentMethod paymentMethod, string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;        

        Cart cartData = GetCartData(userId);

        if (cartData is null || cartData.CartDetails.Count == 0)
            return Result.Failure(CartErrors.CartIsEmpty);

        Restaurant restaurantData = GetRestaurantData(cartData.RestaurantId);
        
        var addOrderResult = await AddOrder(userId, cartData, restaurantData, paymentMethod, specialNotes, promoCodeId, discountAmount);

        if (addOrderResult.IsFailure)
            return Result.Failure(addOrderResult.Error);

        return Result.Success();
    }

    private (string PhoneNumber, string DeliveryAddress, Point DeliveryAddressLocation) GetUserData(string userId)
    {
        var userData = _unitOfWorkRepository.Users
        .GetOneWithSelect(
            expression: u => u.Id == userId,
            tracked: false,
            selector: u => new 
            {
                u.PhoneNumber,
                Address = u.UserAddress
                    .Where(a => a.IsDeliveryAddress)
                    .Select(a => new
                    {
                        a.CustomerAddress,
                        a.Location
                    })
                    .FirstOrDefault()
            }
        )!;
        
        return (userData.PhoneNumber!, userData.Address!.CustomerAddress, userData.Address.Location);
    }

    private Cart GetCartData(string userId)
    {
        var cartData = _unitOfWorkRepository.Carts
            .GetOne(
                expression: c => c.UserId == userId,
                includeProps: [c => c.CartDetails],
                tracked: false
            )!;

        return cartData;
    }

    private Restaurant GetRestaurantData(int restaurantId)
    {
        var restaurantData = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(expression: r => r.Id == restaurantId,
                tracked: false,
                selector: r => new Restaurant
                {
                    DeliveryFee = r.DeliveryFee,
                }
            )!;

        return restaurantData;
    }

    private async Task<Result> AddOrder(string userId, Cart cartData, Restaurant restaurantData, PaymentMethod paymentMethod, string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0)
    {
        (string phoneNumber, string deliveryAddress, Point deliveryAddressLocation) = GetUserData(userId);

        decimal SubPrice = cartData.CartDetails.Select(cd => cd.TotalPrice).Sum();

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            Order order = new()
            {
                UserId = userId,
                RestaurantId = cartData.RestaurantId,
                CustomerPhoneNumber = phoneNumber,
                DeliveryAddress = deliveryAddress,
                DeliveryAddressLocation = deliveryAddressLocation,
                SubPrice = SubPrice,
                DeliveryFee = restaurantData.DeliveryFee,
                ServiceFeePrice = SubPrice * 0.05m,
                DiscountAmount = discountAmount,
                PromoCodeId = promoCodeId,
                Notes = specialNotes,
                Method = paymentMethod,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
            };

            _unitOfWorkRepository.Orders.Add(order);
            _unitOfWorkRepository.SaveChanges();

            // Record promo code usage if applicable
            if (promoCodeId.HasValue && discountAmount > 0)
                _promoCodeService.RecordPromoCodeUsage(promoCodeId.Value, order.Id, userId, discountAmount);

            List<OrderDetails> ordersDetails = [];

            foreach (var cartDetails in cartData.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    OrderId = order.Id,
                    MealId = cartDetails.MealId,
                    MealDetails = cartDetails.MealDetails,
                    MealQuantity = cartDetails.Quantity,
                    MealPrice = cartDetails.MealPrice,
                    ItemsPrice = cartDetails.ItemsPrice,
                    AddOnsPrice = cartDetails.AddOnsPrice
                };

                ordersDetails.Add(orderDetails);
            }

            await _unitOfWorkRepository.OrderDetails.AddRangeAsync(ordersDetails);

            _unitOfWorkRepository.SaveChanges();

            _cartService.Delete(cartData.Id);

            // update AdminDailyAnalytic
            _adminDailyAnalyticsService.InitialUpdate();

            // update RestaurantDailyAnalytic
            _restaurantDailyAnalyticsService.InitialUpdate(order.RestaurantId);

            // update MealsAnalytics
            foreach (var detail in ordersDetails)
            {
                decimal saleAmount = detail.MealPrice + detail.ItemsPrice + detail.AddOnsPrice;
                _mealsAnalyticsService.UpdateSales(order.RestaurantId, detail.MealId, detail.MealQuantity, saleAmount);
            }

            transaction.Commit();

            return Result.Success();
        }
        catch
        {
            transaction.Rollback();

            return Result.Failure(OrderErrors.AddOrderField);
        }
    }

    // DTO classes for deserializing temp order data
    private sealed class TempOrderData
    {
        public string UserId { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public double DeliveryLocationX { get; set; }
        public double DeliveryLocationY { get; set; }
        public decimal SubPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal ServiceFeePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? PromoCodeId { get; set; }
        public string? Notes { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<TempOrderDetailData> OrderDetails { get; set; } = [];
    }

    private sealed class TempOrderDetailData
    {
        public string MealId { get; set; } = string.Empty;
        public string MealDetails { get; set; } = string.Empty;
        public int MealQuantity { get; set; }
        public decimal MealPrice { get; set; }
        public decimal ItemsPrice { get; set; }
        public decimal AddOnsPrice { get; set; }
    }
}
