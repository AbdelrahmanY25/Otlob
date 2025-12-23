namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class MealCategoriesController(IMealCategoryService mealCategoryService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealCategoryService  _mealCategoryService = mealCategoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(MenuCategoryRequest request)
    {
        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));
        
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction("Menu", "Menu", new { restaurantKey });
        }

        var result = _mealCategoryService.Add(restaurantId, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu", new { restaurantKey });
        }

        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(string key, MenuCategoryRequest request)
    {
        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction("Menu", "Menu", new { restaurantKey });
        }

        var result = _mealCategoryService.Update(key, request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu", new { restaurantKey });
        }

        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }

    public IActionResult Delete(string key)
    {
        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session time out try again.";
            return RedirectToAction("index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        var reult = _mealCategoryService.Delete(key);

        if (reult.IsFailure)
        {
            TempData["Error"] = reult.Error.Description;
            return RedirectToAction("Menu", "Menu", new { restaurantKey });
        }

        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }
}
