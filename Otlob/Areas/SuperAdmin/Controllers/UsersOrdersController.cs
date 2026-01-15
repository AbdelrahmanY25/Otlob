namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin)]
[Authorize(Roles = DefaultRoles.SuperAdmin)]
public class UsersOrdersController(
    ICustomerOrdersService customerOrdersService, 
    IOrderDetailsService orderDetailsService,
    IOrderRatingService orderRatingService,
    IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly ICustomerOrdersService _customerOrdersService = customerOrdersService;
    private readonly IOrderDetailsService _orderDetailsService = orderDetailsService;
    private readonly IOrderRatingService _orderRatingService = orderRatingService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Orders(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "User ID is required.";
            return RedirectToAction("Customers", "Users", new { Area = DefaultRoles.SuperAdmin });
        }

        try
        {
            string decryptedUserId = _dataProtector.Unprotect(userId);
            var orders = _customerOrdersService.GetUserOrdersByUserId(decryptedUserId);
            
            ViewBag.UserId = userId;
            return View(orders.ToList());
        }
        catch
        {
            TempData["Error"] = "Invalid user ID.";
            return RedirectToAction("Customers", "Users", new { Area = DefaultRoles.SuperAdmin });
        }
    }

    public IActionResult OrderDetails(int id, string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "User ID is required.";
            return RedirectToAction("Customers", "Users", new { Area = DefaultRoles.SuperAdmin });
        }

        var result = _orderDetailsService.GetOrderDetails(id);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Orders), new { userId });
        }

        ViewBag.UserId = userId;
        return View(result.Value);
    }

    public IActionResult ViewRating(int orderId, string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "User ID is required.";
            return RedirectToAction("Customers", "Users", new { Area = DefaultRoles.SuperAdmin });
        }

        var result = _orderRatingService.GetRatingForAdmin(orderId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Orders), new { userId });
        }

        ViewBag.UserId = userId;
        return View(result.Value);
    }
}
