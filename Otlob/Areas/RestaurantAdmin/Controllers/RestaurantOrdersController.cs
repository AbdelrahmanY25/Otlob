namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class RestaurantOrdersController(IRestaurantOrdersService restaurantOrdersService,
                                        IOrderDetailsService orderDetailsService) : Controller
{
    private readonly IRestaurantOrdersService _restaurantOrdersService = restaurantOrdersService;
    private readonly IOrderDetailsService _orderDetailsService = orderDetailsService;

    public IActionResult InProgress()
    {
        var orders = _restaurantOrdersService.GetInProgressRestaurantOrders();
        return View(orders);
    }
    
    public IActionResult Delivered()
    {
        var orders = _restaurantOrdersService.GetDeliveredRestaurantOrders();
        return View(orders);
    }

    public IActionResult Cancelled()
    {
        var orders = _restaurantOrdersService.GetCancelledRestaurantOrders();
        return View(orders);
    }
    
    public IActionResult Details(int orderKey)
    {
        var result = _orderDetailsService.GetOrderDetails(orderKey);

        if (result.IsFailure)
            return RedirectToAction(nameof(InProgress));

        ViewBag.OrderKey = orderKey;
        return View(result.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(int orderKey, string newStatus)
    {
        if (!Enum.TryParse<OrderStatus>(newStatus, out var status))
            return Json(new { success = false, message = "Invalid status" });

        var result = _restaurantOrdersService.UpdateOrderStatus(orderKey, status);

        if (result.IsFailure)
            return Json(new { success = false, message = result.Error.Description });

        return Json(new { success = true, message = "Order status updated successfully" });
    }

    [HttpGet]
    public IActionResult GetUserInfo(int orderId)
    {
        var userInfo = _restaurantOrdersService.GetOrderUserInfo(orderId);

        if (userInfo is null)
            return NotFound();

        return PartialView("_OrderUserInfoPartial", userInfo);
    }
}