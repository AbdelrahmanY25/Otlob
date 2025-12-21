namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MealCategoriesController(IMealCategoryService mealCategoryService) : Controller
{
    private readonly IMealCategoryService _mealCategoryService = mealCategoryService;

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(MenuCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction("Menu", "Menu");
        }

        var result = _mealCategoryService.Add(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!), request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }            

        return RedirectToAction("Menu", "Menu");
    }
    
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(string key, MenuCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction("Menu", "Menu");
        }

        var result = _mealCategoryService.Update(key, request);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }            

        return RedirectToAction("Menu", "Menu");
    }

    public IActionResult Delete(string key)
    {
        var reult = _mealCategoryService.Delete(key);

        if (reult.IsFailure)
        {
            TempData["Error"] = reult.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        return RedirectToAction("Menu", "Menu");
    }
}
