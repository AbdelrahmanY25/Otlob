namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class MenuController(IMenuCategoryService menuCategoryService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMenuCategoryService _menuCategoryService = menuCategoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Menu(string key)
    {
        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(key));

        var response = _menuCategoryService.GetAllByRestaurantId(restaurantId);
        
        HttpContext.Session.SetString(StaticData.RestaurantId, key);

        return View(response!.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(MenuCategoryRequest request)
    {
        string id = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));
        
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(Menu), new { key = id });
        }

        var result = _menuCategoryService.Add(restaurantId, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Menu), new { key = id });
        }

        return RedirectToAction(nameof(Menu), new { key = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(string key, MenuCategoryRequest request)
    {
        string id = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(Menu), new { key = id });
        }

        var result = _menuCategoryService.Update(key, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Menu), new { key = id });
        }

        return RedirectToAction(nameof(Menu), new { key = id });
    }

    public IActionResult Delete(string key)
    {
        string id = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        var reult = _menuCategoryService.Delete(key);

        if (reult.IsFailure)
        {
            TempData["Error"] = reult.Error.Description;
            return RedirectToAction(nameof(Menu), new { key = id });
        }

        return RedirectToAction(nameof(Menu), new { key = id });
    }
}
