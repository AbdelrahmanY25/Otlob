namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class MenuController(IDataProtectionProvider dataProtectionProvider, IMenuService menuService) : Controller
{
    private readonly IMenuService _menuService = menuService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Menu(string restaurantKey)
    {
        // TODO: Handle Exception
        var response = _menuService.GetMenu(int.Parse(_dataProtector.Unprotect(restaurantKey)));

        HttpContext.Session.SetString(StaticData.RestaurantId, restaurantKey);

        return View(response!.Value);
    }
}
