namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MenuController(IMenuCategoryService menuCategoryService) : Controller
{
    private readonly IMenuCategoryService _menuCategoryService = menuCategoryService;

    public IActionResult Menu()
    {
        var response = _menuCategoryService.GetAllByRestaurantId(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!));
        
        return View(response!.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(MenuCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(Menu));
        }

        var result = _menuCategoryService.Add(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!), request);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Menu));
        }            

        return RedirectToAction(nameof(Menu));
    }
    
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(string key, MenuCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
            return RedirectToAction(nameof(Menu));
        }

        var result = _menuCategoryService.Update(key, request);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Menu));
        }            

        return RedirectToAction(nameof(Menu));
    }

    public IActionResult Delete(string key)
    {
        var reult = _menuCategoryService.Delete(key);

        if (reult.IsFailure)
        {
            TempData["Error"] = reult.Error.Description;
            return RedirectToAction(nameof(Menu));
        }

        return RedirectToAction(nameof(Menu));
    }
}
