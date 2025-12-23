namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class MealAddOnsController(IMealAddOnService mealAddOnService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealAddOnService _mealAddOnService = mealAddOnService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Add(AddOnRequest request)
    {
        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        // TODO: Handle Exception
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var result = _mealAddOnService.Add(request, restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        TempData["Success"] = "AddOn Added Successfully";
        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }

    public IActionResult Update(AddOnRequest request, string key)
    {
        var result = _mealAddOnService.Update(request, key);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        TempData["Success"] = "AddOn Updated Successfully";
        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }

    public IActionResult Delete(string key)
    {
        var result = _mealAddOnService.Delete(key);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        TempData["Success"] = "AddOn Deleted Successfully";
        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }
}
