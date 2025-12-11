namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer)]
public class OrderController(ICartService cartService, IOrderService orderService,
                       IOrderedMealsService orderedMealsService, ITempOrderService tempOrderService) : Controller
{
    private readonly ICartService cartService = cartService;
    private readonly IOrderService orderService = orderService;
    private readonly ITempOrderService tempOrderService = tempOrderService;
    private readonly IOrderedMealsService orderedMealsService = orderedMealsService;

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult PlaceOrder(string id, Order order)
    {
        Cart? cart = cartService.GetCartById(id);

        if (cart is null)
        {
            TempData["Error"] = "Failed to retrieve order details.";
            return RedirectToAction("Index", "Home");
        }

        order.RestaurantId = cart.RestaurantId;
        order.UserId = cart.UserId;
        order.TotalTaxPrice = cart.Restaurant.DeliveryFee;

        return CalculateTotalOrderPrice(cart.Id, order);
    }

    public IActionResult AddOrder(int cartId, Order order)
    {            
        if (order is null)
        {
            TempData["Error"] = "Failed to retrieve order details.";
            return RedirectToAction("Index", "Home");
        }

        bool isOrderAdded = orderService.AddOrder(cartId, order);

        if (isOrderAdded)
        {
            TempData["Success"] = "Your Order Has Been Sent To Restaurant...";
            return RedirectToAction("Index", "Home");
        }

        TempData["Error"] = "Failed to add order.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult CalculateTotalOrderPrice(int cartId, Order order)
    {
        decimal totalMealsPrice = Math.Ceiling(orderedMealsService.CalculateTotalMealsPrice(cartId));

        order.TotalMealsPrice = totalMealsPrice;

        return CheckOnPaymentMethod(cartId, order);
    }

    public IActionResult CheckOnPaymentMethod(int cartId, Order order)
    {
        if (order.Method == PaymentMethod.Credit)
        {
            return PayWithCredit(cartId, order);
        }

        return AddOrder(cartId, order);
    }

    public IActionResult PayWithCredit(int cartId, Order order)
    {
        var meals = orderedMealsService.GetOrderedMealsWithMealsDetails(cartId);

        decimal deliveryFee = order.TotalTaxPrice;

        TempOrder tempOrder = tempOrderService.AddTempOrder(order);

        SessionCreateOptions options = new()
        {
            PaymentMethodTypes = [ "card" ],
            LineItems = [],
            Mode = "payment",
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Order/FinishPaymentProcess/?tempOrderId={tempOrder.Id}&cartId={cartId}",
            CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Home/Index",
        };

        foreach (var item in meals)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "EGP",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Meal.Name,
                        Description = item.Meal.Description,
                    },
                    UnitAmount = (long)Math.Ceiling(item.Meal.Price * 100),
                },
                Quantity = item.Quantity,
            });
        }

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
                UnitAmount = (long)Math.Ceiling(deliveryFee * 100),
            },
            Quantity = 1,
        });

        var service = new SessionService();
        var session = service.Create(options);
        return Redirect(session.Url);
    }

    public IActionResult FinishPaymentProcess(string tempOrderId, int cartId)
    {
        TempOrder tempOrder = tempOrderService.GetTempOrder(tempOrderId)!;

        if (tempOrder is null)
        {
            TempData["Error"] = "Session was expired Please try again.";
            return RedirectToAction("Index", "Home");
        }

        Order order = JsonConvert.DeserializeObject<Order>(tempOrder.OrderData!)!;

        if (order is null)
        {
            TempData["Error"] = "Session was expired P;ease try again.";
            return RedirectToAction("Index", "Home");
        }

        tempOrderService.RemoveTempOrder(tempOrder);

        return AddOrder(cartId, order);
    }
}
