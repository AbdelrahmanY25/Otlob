namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class RestaurantOrdersController(IRestaurantOrdersService restaurantOrdersService,
                                        IOrderDetailsService orderDetailsService) : Controller
{
    private readonly IOrderDetailsService _orderDetailsService = orderDetailsService;
    private readonly IRestaurantOrdersService _restaurantOrdersService = restaurantOrdersService;

    public IActionResult InProgress()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        
        var orders = _restaurantOrdersService.GetAllInProgressByRestaurantId(restaurantId);
        
        return View(orders);
    }
    
    public IActionResult Delivered()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        
        var orders = _restaurantOrdersService.GetAllDeliveredByRestaurantId(restaurantId);
        
        return View(orders);
    }

    public IActionResult Cancelled()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        
        var orders = _restaurantOrdersService.GetAllCancelledByRestaurantId(restaurantId);
        
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

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        var result = _restaurantOrdersService.UpdateOrderStatus(restaurantId, orderKey, status);

        if (result.IsFailure)
            return Json(new { success = false, message = result.Error.Description });

        return Json(new { success = true, message = "Order status updated successfully" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder(int orderId, RestaurantCancelReason reason)
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        var result = _restaurantOrdersService.CancelOrder(restaurantId, orderId, reason);

        if (result.IsFailure)
            return Json(new { success = false, message = result.Error.Description });

        return Json(new { success = true, message = "Order cancelled successfully" });
    }

    [HttpGet]
    public IActionResult GetUserInfo(int orderId)
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        
        var userInfo = _restaurantOrdersService.GetOrderUserInfo(restaurantId, orderId);

        if (userInfo is null)
            return NotFound();

        return PartialView("_OrderUserInfoPartial", userInfo);
    }
}