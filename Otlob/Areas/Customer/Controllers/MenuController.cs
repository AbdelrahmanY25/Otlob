namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class MenuController(IMenuService menuService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMenuService _menuService = menuService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Menu(string restaurantKey)
    {
        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var response = _menuService.MenuForCustomer(restaurantId);

        if (response.IsFailure)
        {
            TempData["Error"] = response.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.Customer });
        }

        return View(response.Value);
    }

    public IActionResult Meal(string mealKey)
    {
        var response = _menuService.GetMeal(mealKey);

        if (response.IsFailure)
        {
            TempData["Error"] = response.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.Customer });
        }

        return View(response.Value);
    }
}
