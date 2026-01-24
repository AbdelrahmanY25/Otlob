namespace Otlob.ApiServices;

public class ApiOrdersService(
    IUnitOfWorkRepository unitOfWorkRepository,
    IDataProtectionProvider dataProtectionProvider,
    IHttpContextAccessor httpContextAccessor,
    IPromoCodeService promoCodeService,
    IAdminDailyAnalyticsService adminDailyAnalyticsService,
    IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
    IMealsAnalyticsService mealsAnalyticsService,
    ITempOrderService tempOrderService) : IApiOrdersService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IPromoCodeService _promoCodeService = promoCodeService;
    private readonly IAdminDailyAnalyticsService _adminDailyAnalyticsService = adminDailyAnalyticsService;
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;
    private readonly IMealsAnalyticsService _mealsAnalyticsService = mealsAnalyticsService;
    private readonly ITempOrderService _tempOrderService = tempOrderService;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<OrderResponse> PlaceOrderAsync(OrderRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;

        if (request.Items == null || !request.Items.Any())
            return CreateErrorResponse(OrderApiErrors.EmptyOrder);

        int restaurantId;
        
        try
        {
            restaurantId = int.Parse(_dataProtector.Unprotect(request.RestaurantKey));
        }
        catch
        {
            return CreateErrorResponse(OrderApiErrors.RestaurantNotFound);
        }

        var restaurant = _unitOfWorkRepository.Restaurants
            .GetOneWithSelect(
                expression: r => r.Id == restaurantId && r.AcctiveStatus == AcctiveStatus.Acctive,
                tracked: false,
                selector: r => new { r.Id, r.DeliveryFee, r.MinimumOrderPrice, r.IsOpen }
            );

        if (restaurant is null)
            return CreateErrorResponse(OrderApiErrors.RestaurantNotFound);

        int addressId;
        
        try
        {
            addressId = int.Parse(_dataProtector.Unprotect(request.AddressKey));
        }
        catch
        {
            return CreateErrorResponse(OrderApiErrors.AddressNotFound);
        }

        var address = _unitOfWorkRepository.Addresses
            .GetOneWithSelect(
                expression: a => a.Id == addressId && a.UserId == userId,
                tracked: false,
                selector: a => new { a.CustomerAddress, a.Location, a.PhoneNumber }
            );

        if (address is null)
            return CreateErrorResponse(OrderApiErrors.AddressNotFound);

        // Get user phone number
        var user = _unitOfWorkRepository.Users
            .GetOneWithSelect(
                expression: u => u.Id == userId,
                tracked: false,
                selector: u => new { u.PhoneNumber, u.Email, u.StripeCustomerId }
            );

        string phoneNumber = address.PhoneNumber ?? user?.PhoneNumber ?? "";

        // Validate and calculate order details
        var orderDetailsResult = await ValidateAndCalculateOrderDetailsAsync(request.Items, restaurantId);
        
        if (!orderDetailsResult.IsSuccess)
            return CreateErrorResponse(orderDetailsResult.Error!);

        var resultData = orderDetailsResult.Data!.Value;
        var orderDetails = resultData.OrderDetails;
        var subTotal = resultData.SubTotal;
        var mealDetailsForStripe = resultData.MealDetailsForStripe;
        
        // Calculate fees
        decimal serviceFee = subTotal * 0.05m;
        decimal discountAmount = 0;
        int? promoCodeId = null;

        decimal orderAmount = subTotal + restaurant.DeliveryFee + serviceFee;

        // Validate promo code if provided
        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            var promoResult = _promoCodeService.ValidateAndCalculateDiscount(request.PromoCode, restaurantId, orderAmount, userId);

            if (promoResult.IsSuccess)
            {
                discountAmount = promoResult.Value.DiscountAmount;
                promoCodeId = promoResult.Value.PromoCodeId;
            }
            // If promo code is invalid, we continue without discount (you can also return error if preferred)
        }

        decimal totalPrice = subTotal + restaurant.DeliveryFee + serviceFee - discountAmount;

        // Handle payment method
        if (request.PaymentMethod == PaymentMethod.Cash)
        {
            return await PlaceCashOrderAsync(
                userId, restaurantId, phoneNumber, address.CustomerAddress, address.Location,
                subTotal, restaurant.DeliveryFee, serviceFee, discountAmount, promoCodeId,
                request.SpecialNotes, orderDetails, totalPrice
            );
        }
        else // Credit
        {
            return await CreateStripePaymentIntentAsync(
                userId, restaurantId, phoneNumber, address.CustomerAddress, address.Location,
                subTotal, restaurant.DeliveryFee, serviceFee, discountAmount, promoCodeId,
                request.SpecialNotes, orderDetails, totalPrice, mealDetailsForStripe,
                user?.Email, user?.StripeCustomerId
            );
        }
    }

    public async Task<OrderResponse> ConfirmCreditPaymentAsync(string tempOrderId)
    {
        TempOrder? tempOrder = _tempOrderService.GetTempOrder(tempOrderId);

        if (tempOrder is null)
            return CreateErrorResponse(OrderApiErrors.InvalidPaymentSession);

        // Deserialize the stored order data
        var orderData = JsonSerializer.Deserialize<TempOrderData>(tempOrder.OrderData!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (orderData is null)
            return CreateErrorResponse(OrderApiErrors.InvalidPaymentSession);

        // Verify payment was completed by checking payment intent status
        try
        {
            var paymentIntentService = new Stripe.PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(orderData.PaymentIntentId);

            if (paymentIntent.Status != "succeeded")
                return CreateErrorResponse(OrderApiErrors.PaymentNotCompleted);
        }
        catch
        {
            return CreateErrorResponse(OrderApiErrors.PaymentNotCompleted);
        }

        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
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
                Method = PaymentMethod.Credit,
                OrderDate = orderData.OrderDate,
                Status = OrderStatus.Pending,
            };

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

            // Remove temp order
            _tempOrderService.RemoveTempOrder(tempOrder);

            // Update analytics
            _adminDailyAnalyticsService.InitialUpdate();
            _restaurantDailyAnalyticsService.InitialUpdate(order.RestaurantId);

            foreach (var detail in orderDetailsList)
            {
                decimal saleAmount = detail.MealPrice + detail.ItemsPrice + detail.AddOnsPrice;
                _mealsAnalyticsService.UpdateSales(order.RestaurantId, detail.MealId, detail.MealQuantity, saleAmount);
            }

            transaction.Commit();

            return new OrderResponse(
                IsSuccess: true,
                Message: "Order placed successfully. Payment received!",
                OrderId: order.Id,
                Status: order.Status,
                TotalAmount: orderData.TotalPrice
            );
        }
        catch
        {
            transaction.Rollback();
            return CreateErrorResponse(OrderApiErrors.OrderCreationFailed);
        }
    }

    #region Private Methods

    private async Task<OrderResponse> PlaceCashOrderAsync(
        string userId, int restaurantId, string phoneNumber, string deliveryAddress,
        Point deliveryLocation, decimal subPrice, decimal deliveryFee, decimal serviceFee,
        decimal discountAmount, int? promoCodeId, string? notes,
        List<OrderDetailData> orderDetails, decimal totalPrice)
    {
        using var transaction = _unitOfWorkRepository.BeginTransaction();

        try
        {
            Order order = new()
            {
                UserId = userId,
                RestaurantId = restaurantId,
                CustomerPhoneNumber = phoneNumber,
                DeliveryAddress = deliveryAddress,
                DeliveryAddressLocation = deliveryLocation,
                SubPrice = subPrice,
                DeliveryFee = deliveryFee,
                ServiceFeePrice = serviceFee,
                DiscountAmount = discountAmount,
                PromoCodeId = promoCodeId,
                Notes = notes,
                Method = PaymentMethod.Cash,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
            };

            _unitOfWorkRepository.Orders.Add(order);
            _unitOfWorkRepository.SaveChanges();

            // Record promo code usage if applicable
            if (promoCodeId.HasValue && discountAmount > 0)
                _promoCodeService.RecordPromoCodeUsage(promoCodeId.Value, order.Id, userId, discountAmount);

            List<OrderDetails> ordersDetailsList = orderDetails.Select(od => new OrderDetails
            {
                OrderId = order.Id,
                MealId = od.MealId,
                MealDetails = od.MealDetails,
                MealQuantity = od.MealQuantity,
                MealPrice = od.MealPrice,
                ItemsPrice = od.ItemsPrice,
                AddOnsPrice = od.AddOnsPrice
            }).ToList();

            await _unitOfWorkRepository.OrderDetails.AddRangeAsync(ordersDetailsList);
            _unitOfWorkRepository.SaveChanges();

            // Update analytics
            _adminDailyAnalyticsService.InitialUpdate();
            _restaurantDailyAnalyticsService.InitialUpdate(order.RestaurantId);

            foreach (var detail in ordersDetailsList)
            {
                decimal saleAmount = detail.MealPrice + detail.ItemsPrice + detail.AddOnsPrice;
                _mealsAnalyticsService.UpdateSales(order.RestaurantId, detail.MealId, detail.MealQuantity, saleAmount);
            }

            transaction.Commit();

            return new OrderResponse(
                IsSuccess: true,
                Message: "Order placed successfully!",
                OrderId: order.Id,
                Status: order.Status,
                TotalAmount: totalPrice
            );
        }
        catch
        {
            transaction.Rollback();
            return CreateErrorResponse(OrderApiErrors.OrderCreationFailed);
        }
    }

    private async Task<OrderResponse> CreateStripePaymentIntentAsync(
        string userId, int restaurantId, string phoneNumber, string deliveryAddress,
        Point deliveryLocation, decimal subPrice, decimal deliveryFee, decimal serviceFee,
        decimal discountAmount, int? promoCodeId, string? notes,
        List<OrderDetailData> orderDetails, decimal totalPrice,
        List<MealDetailForStripe> mealDetailsForStripe, string? userEmail, string? existingStripeCustomerId)
    {
        try
        {
            // Create or get Stripe Customer for the mobile app
            string stripeCustomerId = existingStripeCustomerId ?? "";

            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                var customerService = new Stripe.CustomerService();
                
                var customer = await customerService.CreateAsync(new Stripe.CustomerCreateOptions
                {
                    Email = userEmail,
                    Metadata = new Dictionary<string, string> { { "userId", userId } }
                });
                
                stripeCustomerId = customer.Id;

                // Store the Stripe customer ID for future use
                var user = _unitOfWorkRepository.Users.GetOne(expression: u => u.Id == userId);
                
                if (user is not null)
                {
                    user.StripeCustomerId = stripeCustomerId;
                    _unitOfWorkRepository.SaveChanges();
                }
            }

            // Create Ephemeral Key for the customer (required for Stripe Mobile SDK)
            // Ephemeral keys require a specific Stripe API version
            var ephemeralKeyService = new Stripe.EphemeralKeyService();
            var ephemeralKeyOptions = new Stripe.EphemeralKeyCreateOptions
            {
                Customer = stripeCustomerId,
            };
            
            // Set API version for ephemeral key
            var requestOptions = new Stripe.RequestOptions();
            requestOptions.ApiKey = Stripe.StripeConfiguration.ApiKey;
            var ephemeralKey = await ephemeralKeyService.CreateAsync(ephemeralKeyOptions, requestOptions);

            // Create Payment Intent
            long amountInCents = (long)Math.Ceiling(totalPrice * 100);
            
            var paymentIntentService = new Stripe.PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new Stripe.PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "egp",
                Customer = stripeCustomerId,
                AutomaticPaymentMethods = new Stripe.PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId },
                    { "restaurantId", restaurantId.ToString() }
                },
                Description = $"Order from Restaurant ID: {restaurantId}"
            });

            // Create temp order to store data during payment
            Order tempOrder = new()
            {
                UserId = userId,
                RestaurantId = restaurantId,
                CustomerPhoneNumber = phoneNumber,
                DeliveryAddress = deliveryAddress,
                DeliveryAddressLocation = deliveryLocation,
                SubPrice = subPrice,
                DeliveryFee = deliveryFee,
                ServiceFeePrice = serviceFee,
                DiscountAmount = discountAmount,
                PromoCodeId = promoCodeId,
                Notes = notes,
                Method = PaymentMethod.Credit,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderDetails = orderDetails.Select(od => new OrderDetails
                {
                    MealId = od.MealId,
                    MealDetails = od.MealDetails,
                    MealQuantity = od.MealQuantity,
                    MealPrice = od.MealPrice,
                    ItemsPrice = od.ItemsPrice,
                    AddOnsPrice = od.AddOnsPrice
                }).ToList()
            };

            // Store temp order with payment intent ID
            TempOrder storedTempOrder = AddTempOrderWithPaymentIntent(tempOrder, paymentIntent.Id, totalPrice);

            return new OrderResponse(
                IsSuccess: true,
                Message: "Payment session created. Complete payment to place order.",
                TotalAmount: totalPrice,
                PaymentIntentClientSecret: paymentIntent.ClientSecret,
                EphemeralKey: ephemeralKey.Secret,
                CustomerId: stripeCustomerId,
                TempOrderId: storedTempOrder.Id
            );
        }
        catch (Exception)
        {
            return CreateErrorResponse(OrderApiErrors.StripePaymentFailed);
        }
    }






    private TempOrder AddTempOrderWithPaymentIntent(Order order, string paymentIntentId, decimal totalPrice)
    {
        // Create a serializable representation of the order with payment intent
        var orderData = new
        {
            order.UserId,
            order.RestaurantId,
            order.CustomerPhoneNumber,
            order.DeliveryAddress,
            DeliveryLocationX = order.DeliveryAddressLocation?.X ?? 0,
            DeliveryLocationY = order.DeliveryAddressLocation?.Y ?? 0,
            order.SubPrice,
            order.DeliveryFee,
            order.ServiceFeePrice,
            order.DiscountAmount,
            order.PromoCodeId,
            order.Notes,
            order.Method,
            order.OrderDate,
            order.Status,
            PaymentIntentId = paymentIntentId,
            TotalPrice = totalPrice,
            OrderDetails = order.OrderDetails.Select(m => new
            {
                m.MealId,
                m.MealDetails,
                m.MealQuantity,
                m.MealPrice,
                m.ItemsPrice,
                m.AddOnsPrice
            }).ToList()
        };

        TempOrder tempOrder = new() { OrderData = JsonSerializer.Serialize(orderData, _jsonOptions) };

        _unitOfWorkRepository.TempOrders.Add(tempOrder);
        _unitOfWorkRepository.SaveChanges();

        BackgroundJob.Schedule(() => RemoveTempOrderAfterExpiry(tempOrder.Id), TimeSpan.FromMinutes(20));

        return tempOrder;
    }

    public void RemoveTempOrderAfterExpiry(string tempOrderId)
    {
        bool isTempOrderStillExist = _unitOfWorkRepository.TempOrders.IsExist(to => to.Id == tempOrderId);

        if (isTempOrderStillExist)
        {
            TempOrder theExpiredTempOrder = _unitOfWorkRepository.TempOrders.GetOne(expression: to => to.Id == tempOrderId, ignoreQueryFilter: true)!;
            _unitOfWorkRepository.TempOrders.HardDelete(theExpiredTempOrder);
            _unitOfWorkRepository.SaveChanges();
        }
    }

    private async Task<OrderDetailsValidationResult> ValidateAndCalculateOrderDetailsAsync(IEnumerable<OrderDetailRequest> items, int restaurantId)
    {
        var orderDetails = new List<OrderDetailData>();
        
        var mealDetailsForStripe = new List<MealDetailForStripe>();
        
        decimal subTotal = 0;

        foreach (var item in items)
        {
            if (item.Quantity < 1)
                return OrderDetailsValidationResult.Fail(OrderApiErrors.InvalidQuantity);

            // Get meal with option groups and add-ons
            var meal = _unitOfWorkRepository.Meals
                .GetOneWithSelect(
                    expression: m => m.Id == item.MealKey && m.IsAvailable,
                    tracked: false,
                    selector: m => new
                    {
                        m.Id,
                        m.RestaurantId,
                        m.Name,
                        m.Price,
                        m.OfferPrice,
                        m.HasOffer,
                        OptionGroups = m.OptionGroups.Select(og => new
                        {
                            og.MealOptionGroupId,
                            og.Name,
                            Items = og.OptionItems.Select(oi => new
                            {
                                oi.MealOptionItemId,
                                oi.Name,
                                oi.Price
                            }).ToList()
                        }).ToList(),
                        AddOns = m.MealAddOns.Select(ma => new
                        {
                            ma.AddOn.Id,
                            ma.AddOn.Name,
                            ma.AddOn.Price
                        }).ToList()
                    }
                );

            if (meal is null)
                return OrderDetailsValidationResult.Fail(OrderApiErrors.MealNotFound);

            if (meal.RestaurantId != restaurantId)
                return OrderDetailsValidationResult.Fail(OrderApiErrors.MealNotFromRestaurant);

            // Calculate meal price
            decimal mealPrice = meal.HasOffer ? meal.OfferPrice : meal.Price;

            // Calculate option items price
            decimal itemsPrice = 0;
            var selectedItems = new List<SelectedItemInfo>();

            foreach (var optionItemKey in item.SelectedOptionItems ?? [])
            {
                var optionItem = meal.OptionGroups
                    .SelectMany(og => og.Items)
                    .FirstOrDefault(oi => oi.MealOptionItemId == optionItemKey);

                if (optionItem is null)
                    return OrderDetailsValidationResult.Fail(OrderApiErrors.InvalidOptionItem);

                itemsPrice += optionItem.Price;
                selectedItems.Add(new SelectedItemInfo { Id = optionItem.MealOptionItemId, Name = optionItem.Name, Price = optionItem.Price });
            }

            // Calculate add-ons price
            decimal addOnsPrice = 0;
            var selectedAddOns = new List<SelectedItemInfo>();

            foreach (var addOnKey in item.SelectedAddons ?? [])
            {                
                var addOn = meal.AddOns.FirstOrDefault(a => a.Id == addOnKey);

                if (addOn is null)
                    return OrderDetailsValidationResult.Fail(OrderApiErrors.InvalidAddOn);

                addOnsPrice += addOn.Price;
                selectedAddOns.Add(new SelectedItemInfo { Id = addOn.Id, Name = addOn.Name, Price = addOn.Price });
            }

            // Create meal details JSON
            var mealDetailsJson = JsonSerializer.Serialize(new
            {
                items = selectedItems,
                addOns = selectedAddOns,
                note = item.Note
            }, _jsonOptions);

            decimal lineTotal = (mealPrice + itemsPrice + addOnsPrice) * item.Quantity;
            subTotal += lineTotal;

            orderDetails.Add(new OrderDetailData
            {
                MealId = meal.Id,
                MealDetails = mealDetailsJson,
                MealQuantity = item.Quantity,
                MealPrice = mealPrice,
                ItemsPrice = itemsPrice,
                AddOnsPrice = addOnsPrice
            });

            mealDetailsForStripe.Add(new MealDetailForStripe
            {
                MealName = meal.Name,
                UnitPrice = mealPrice + itemsPrice + addOnsPrice,
                Quantity = item.Quantity,
                Description = BuildMealDescription(selectedItems, selectedAddOns)
            });
        }

        return OrderDetailsValidationResult.Success(orderDetails, subTotal, mealDetailsForStripe);
    }

    private static string BuildMealDescription(List<SelectedItemInfo> items, List<SelectedItemInfo> addOns)
    {
        var combined = items.Select(i => i.Name).Concat(addOns.Select(a => a.Name));
        return string.Join(", ", combined);
    }

    private static OrderResponse CreateErrorResponse(ApiError error)
    {
        return new OrderResponse(
            IsSuccess: false,
            Message: error.Description
        );
    }

    #endregion

    #region DTOs

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
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
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

    private sealed class OrderDetailData
    {
        public string MealId { get; set; } = string.Empty;
        public string MealDetails { get; set; } = string.Empty;
        public int MealQuantity { get; set; }
        public decimal MealPrice { get; set; }
        public decimal ItemsPrice { get; set; }
        public decimal AddOnsPrice { get; set; }
    }

    private sealed class MealDetailForStripe
    {
        public string MealName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    private sealed class SelectedItemInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    private sealed class OrderDetailsValidationResult
    {
        public bool IsSuccess { get; private set; }
        public ApiError? Error { get; private set; }
        public (List<OrderDetailData> OrderDetails, decimal SubTotal, List<MealDetailForStripe> MealDetailsForStripe)? Data { get; private set; }

        public static OrderDetailsValidationResult Fail(ApiError error) => new() { IsSuccess = false, Error = error };
        public static OrderDetailsValidationResult Success(List<OrderDetailData> orderDetails, decimal subTotal, List<MealDetailForStripe> mealDetailsForStripe)
            => new() { IsSuccess = true, Data = (orderDetails, subTotal, mealDetailsForStripe) };
    }

    #endregion
}
