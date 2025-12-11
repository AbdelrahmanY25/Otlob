namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin), Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class MealsController(IMealService mealService, IMenuCategoryService menuCategoryService,
                             IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealService _mealService = mealService;
    private readonly IMenuCategoryService _menuCategoryService = menuCategoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Index()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);
        
        var mealsResult = _mealService.GetAllByRestaurantId(restaurantId)!;       

        return View(mealsResult.Value);
    }

    public IActionResult Add()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        var categoriesResult = _menuCategoryService.GetAllByRestaurantId(restaurantId);

        if (categoriesResult!.IsFailure)
        {
            TempData["Error"] = categoriesResult.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        MealRequest request = new() { Categories = categoriesResult.Value };

        return View(request);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(MealRequest request, UploadImageRequest imageRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        Result result = _mealService.Add(restaurantId, request, imageRequest);

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
            return RedirectToAction(nameof(Index));
        }

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        return View(resultResponse.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Update(MealRequest request, string key)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Update), new { key });
        }

        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        Result isMealAdded = _mealService.Update(request, key, restaurantId);

        if (isMealAdded.IsFailure)
        {
            TempData["Error"] = isMealAdded.Error.Description;
            return RedirectToAction(nameof(Update), new { key });
        }

        return BackToMealsView("Meal updated succefully");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateImage(UploadImageRequest imageRequest, string key)
    {        
        Result changeMealImageResult = _mealService.ChangeMealImage(imageRequest.Image, key);

        if (changeMealImageResult.IsFailure)
        {
            TempData["Error"] = changeMealImageResult.Error.Description;
            return RedirectToAction(nameof(Update), new { key });
        }

        TempData["Success"] = "Meal image Updated successfully";
        return RedirectToAction(nameof(Update), new { key });
    }

    public IActionResult DeletedMeals()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return RedirectToAction("Login", "Account", new { Area = "Customer" });
        }

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
    public IActionResult DeleteMeal(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var result = _mealService.DeleteMeal(mealId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index");
    }

    public IActionResult UnDeleteMeal(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));
        
        var result = _mealService.UnDeleteMeal(mealId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("DeletedMeals");
    }

    private RedirectToActionResult BackToMealsView(string msg)
    {
        TempData["Success"] = msg;
        return RedirectToAction(nameof(Index));
    }
}
