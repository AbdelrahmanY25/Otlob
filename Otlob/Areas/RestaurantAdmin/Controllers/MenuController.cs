namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MenuController(IMenuService menuService) : Controller
{
    private readonly IMenuService _menuService = menuService;

    public IActionResult Menu()
    {
        var response = _menuService.GetMenu(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!));

        if (response.IsFailure)
        {
            TempData["Error"] = response.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        return View(response!.Value);
    }
}
