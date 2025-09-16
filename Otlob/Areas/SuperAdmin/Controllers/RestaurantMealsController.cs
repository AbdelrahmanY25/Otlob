namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole)]
public class RestaurantMealsController(IMealService mealService,
                                IImageService imageService,
                                 IDataProtectionProvider dataProtectionProvider,
                                 IMealPriceHistoryService mealPriceHistoryService) : Controller
{
    private readonly IMealService _mealService = mealService;
    private readonly IImageService _imageService = imageService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;

    public IActionResult RestaurantMeals(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));
        
        HttpContext.Session.SetString("restaurantId", _dataProtector.Protect(restaurantId.ToString()));
        
        var result = _mealService.GetMealsByRestaurantId(restaurantId);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        return View(result.Value);
    }

    public IActionResult AddMeal() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddMeal(MealVm mealVM, IFormFile image)
    {
        if (!ModelState.IsValid)
        {
            return View(mealVM);
        }

        int restaurantId = int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!));

        Result isMealAdded = _mealService.AddMeal(mealVM, restaurantId, image);

        if (isMealAdded.IsFailure)
        {
            ModelState.AddModelError("", isMealAdded.Error.Description);
            return View(mealVM);
        }

        return RedirectToAction(nameof(RestaurantMeals), new { id = _dataProtector.Protect(restaurantId.ToString()) });
    }

    public IActionResult MealDetails(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var result = _mealService.GetMealVM(mealId);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        HttpContext.Session.SetString("MealId", _dataProtector.Protect(mealId.ToString()));

        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult MealDetails(MealVm mealVM)
    {
        if (!ModelState.IsValid)
        {
            return View(mealVM);
        }

        int mealId = int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString("MealId")!));

        Result isMealAdded = _mealService.EditMeal(mealVM, mealId);

        if (isMealAdded.IsSuccess)
        {
            TempData["Success"] = "Meal updated successfully";
            return RedirectToAction("MealDetails", new { id = _dataProtector.Protect(mealId.ToString()) });
        }

        if (isMealAdded.Error.Equals(MealErrors.MealNotFound))
        {
            TempData["Error"] = isMealAdded.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        TempData["Error"] = isMealAdded.Error.Description;
        return View(mealVM);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult MealImage(IFormFile image)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(HttpContext!.Session.GetString("MealId")!));

        Result changeMealImageResult = _mealService.ChangeMealImage(image, mealId);

        if (changeMealImageResult.IsSuccess)
        {
            TempData["Success"] = "Meal image Updated successfully";
            return RedirectToAction("MealDetails", new { id = _dataProtector.Protect(mealId.ToString()) });
        }

        if (changeMealImageResult.Error.Equals(MealErrors.MealNotFound))
        {
            TempData["Error"] = changeMealImageResult.Error.Description;
            return RedirectToAction("Index", "Home");
        }

        TempData["Error"] = changeMealImageResult.Error.Description;
        return RedirectToAction("MealDetails", new { id = _dataProtector.Protect(mealId.ToString()) });
    }

    public IActionResult MealPriceHistoryDetails(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var mealPriceHistories = _mealPriceHistoryService.GetMealPriceHistories(mealId);

        return View(mealPriceHistories);
    }

    public IActionResult RestaurantDeletedMeals(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        HttpContext.Session.SetString("restaurantId", _dataProtector.Protect(restaurantId.ToString()));

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

        int restaurantId = (int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!)));

        return RedirectToAction("RestaurantMeals", "RestaurantMeals", new { id = _dataProtector.Protect(restaurantId.ToString()) });
    }

    public IActionResult UnDeleteMeal(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var result = _mealService.DeleteMeal(mealId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home");
        }

        int restaurantId = int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!));

        return RedirectToAction("RestaurantDeletedMeals", "RestaurantMeals", new { id = _dataProtector.Protect(restaurantId.ToString()   ) });
    }
}
