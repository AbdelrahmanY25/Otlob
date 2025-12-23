namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MealsController(IMealService mealService, IMealCategoryService menuCategoryService,
                             IMealAddOnService mealAddOnService) : Controller
{
    private readonly IMealService _mealService = mealService;
    private readonly IMealCategoryService _menuCategoryService = menuCategoryService;
    private readonly IMealAddOnService _mealAddOnService = mealAddOnService;

    //public IActionResult Index()
    //{        
    //    var mealsResult = _mealService.GetAllByRestaurantId(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!))!;       

    //    return View(mealsResult.Value);
    //}

    public IActionResult Add()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        // get all categories for the restaurant
        var categoriesResult = _menuCategoryService.GetAllByRestaurantId(restaurantId);

        if (categoriesResult!.IsFailure)
        {
            TempData["Error"] = categoriesResult.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        // get all AddOns for the restaurant
        var addOnsResult = _mealAddOnService.GetAllByRestaurantId(restaurantId);

        if (addOnsResult.IsFailure)
        {
            TempData["Error"] = addOnsResult.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        MealRequest request = new() { Categories = categoriesResult.Value, AddOns = addOnsResult.Value };

        return View(request);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(MealRequest request, UploadImageRequest imageRequest)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Add));
        }

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        Result result = await _mealService.AddAsync(restaurantId, request, imageRequest);

        if (result.IsFailure)
        {
            ModelState.AddModelError("", result.Error.Description);
            return View(request);
        }

        return BackToMealsView("Meal Added Succefully");
    }

    public IActionResult Update(string key)
    {
        var resultResponse = _mealService.GetForUpdate(key);

        if (resultResponse.IsFailure)
        {
            TempData["Error"] = resultResponse.Error.Description;
            return RedirectToAction("Menu", "Menu");
        }

        return View(resultResponse.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(MealRequest request, string key)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Update), new { key });
        }

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        Result isMealAdded = await _mealService.UpdateAsync(request, key, restaurantId);

        if (isMealAdded.IsFailure)
        {
            TempData["Error"] = isMealAdded.Error.Description;
            return RedirectToAction(nameof(Update), new { key });
        }

        TempData["Success"] = "Meal updated succefully";
        return RedirectToAction(nameof(Update), new { key });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateImage(UploadImageRequest imageRequest, string key)
    {        
        Result changeMealImageResult = _mealService.UpdateMealImage(imageRequest, key);

        if (changeMealImageResult.IsFailure)
        {
            TempData["Error"] = changeMealImageResult.Error.Description;
            return RedirectToAction(nameof(Update), new { key });
        }

        TempData["Success"] = "Meal image Updated successfully";
        return RedirectToAction(nameof(Update), new { key });
    }

    public IActionResult Deleted()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        var result = _mealService.GetDeletedMeals(restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(string id)
    {
        var result = _mealService.DeleteMeal(id);

        if (result.IsFailure)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction("Menu", "Menu");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UnDelete(string id)
    {        
        var result = _mealService.UnDeleteMeal(id);

        if (result.IsFailure)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction(nameof(Deleted));
    }

    private RedirectToActionResult BackToMealsView(string msg)
    {
        TempData["Success"] = msg;
        return RedirectToAction("Menu", "Menu");
    }
}
