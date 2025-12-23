namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MealAddOnsController(IMealAddOnService mealAddOnService) : Controller
{
    private readonly IMealAddOnService _mealAddOnService = mealAddOnService;

    public IActionResult Add(AddOnRequest request)
    {
        var result = _mealAddOnService.Add(request, int.Parse(User.FindFirstValue(StaticData.RestaurantId)!));

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        TempData["Success"] = "AddOn Added Successfully";
        return RedirectToAction("Menu", "Menu");        
    }

    public IActionResult Update(AddOnRequest request, string key)
    {
        var result = _mealAddOnService.Update(request, key);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        TempData["Success"] = "AddOn Updated Successfully";
        return RedirectToAction("Menu", "Menu");
    }

    public IActionResult Delete(string key)
    {
        var result =_mealAddOnService.Delete(key);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        TempData["Success"] = "AddOn Deleted Successfully";
        return RedirectToAction("Menu", "Menu");
    }
}
