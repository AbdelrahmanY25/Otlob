namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class RestaurantOrdersController(IRestaurantOrdersService restaurantOrdersService,
                                        IOrderDetailsService orderDetailsService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IOrderDetailsService _orderDetailsService = orderDetailsService;
    private readonly IRestaurantOrdersService _restaurantOrdersService = restaurantOrdersService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult InProgress(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        int? sessionValue = HttpContext.Session.GetInt32(StaticData.RestaurantId);

        if (sessionValue is null)
            HttpContext.Session.SetInt32(StaticData.RestaurantId, restaurantId);

        var orders = _restaurantOrdersService.GetAllInProgressByRestaurantId(restaurantId);

        return View(orders);
    }

    public IActionResult Delivered(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        int? sessionValue = HttpContext.Session.GetInt32(StaticData.RestaurantId);

        if (sessionValue is null)
            HttpContext.Session.SetInt32(StaticData.RestaurantId, restaurantId);

        var orders = _restaurantOrdersService.GetAllDeliveredByRestaurantId(restaurantId);

        return View(orders);
    }

    public IActionResult Cancelled(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        int? sessionValue = HttpContext.Session.GetInt32(StaticData.RestaurantId);

        if (sessionValue is null)
            HttpContext.Session.SetInt32(StaticData.RestaurantId, restaurantId);

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

        int? sessionValue = HttpContext.Session.GetInt32(StaticData.RestaurantId);

        if (sessionValue is null)
            return Json(new { success = false, message = "Seesion timeout try reload the page." });                   
            
        var result = _restaurantOrdersService.UpdateOrderStatus((int)sessionValue, orderKey, status);

        if (result.IsFailure)
            return Json(new { success = false, message = result.Error.Description });

        return Json(new { success = true, message = "Order status updated successfully" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder(int orderId, RestaurantCancelReason reason)
    {
        int? sessionValue = HttpContext.Session.GetInt32(StaticData.RestaurantId);

        if (sessionValue is null)
            return Json(new { success = false, message = "Seesion timeout try reload the page." });

        var result = _restaurantOrdersService.CancelOrder((int)sessionValue, orderId, reason);

        if (result.IsFailure)
            return Json(new { success = false, message = result.Error.Description });

        return Json(new { success = true, message = "Order cancelled successfully" });
    }

    [HttpGet]
    public IActionResult GetUserInfo(int orderId)
    {
        int? sessionValue = HttpContext.Session.GetInt32(StaticData.RestaurantId);

        if (sessionValue is null)
            return Json(new { success = false, message = "Seesion timeout try reload the page." });

        var userInfo = _restaurantOrdersService.GetOrderUserInfo((int)sessionValue, orderId);

        if (userInfo is null)
            return NotFound();

        return PartialView("_OrderUserInfoPartial", userInfo);
    }
}
