namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin)]
public class RestaurantMealsController(IMealService mealService,
                                IFileService imageService,
                                 IDataProtectionProvider dataProtectionProvider,
                                 IMealPriceHistoryService mealPriceHistoryService) : Controller
{
    private readonly IMealService _mealService = mealService;
    private readonly IFileService _imageService = imageService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;

    public IActionResult RestaurantMeals(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));
        
        HttpContext.Session.SetString(StaticData.RestaurantId, id);
        
        var result = _mealService.GetAllByRestaurantId(restaurantId);
        
        if (result!.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        return View(result.Value);
    }

    public IActionResult AddMeal() => View();

    //[HttpPost, ValidateAntiForgeryToken]
    //public IActionResult AddMeal(MealVm mealVM, IFormFile image)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(mealVM);
    //    }

    //    int restaurantId = int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString(StaticData.RestaurantId)!));

    //    Result isMealAdded = _mealService.Add(mealVM, restaurantId, image);

    //    if (isMealAdded.IsFailure)
    //    {
    //        ModelState.AddModelError("", isMealAdded.Error.Description);
    //        return View(mealVM);
    //    }

    //    return RedirectToAction(nameof(RestaurantMeals), new { id = HttpContext.Session.GetString(StaticData.RestaurantId) });
    //}

    public IActionResult MealDetails(string key)
    {
        var result = _mealService.GetForUpdate(key);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        HttpContext.Session.SetString(StaticData.MealId, key);

        return View(result.Value);
    }

    //[HttpPost, ValidateAntiForgeryToken]
    //public IActionResult MealDetails(MealVm mealVM)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(mealVM);
    //    }

    //    int mealId = int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString(StaticData.MealId)!));

    //    Result isMealAdded = _mealService.Update(mealVM, mealId);

    //    if (isMealAdded.IsSuccess)
    //    {
    //        TempData["Success"] = "Meal updated successfully";
    //        return RedirectToAction("MealDetails", new { id = _dataProtector.Protect(mealId.ToString()) });
    //    }

    //    if (isMealAdded.Error.Equals(MealErrors.MealNotFound))
    //    {
    //        TempData["Error"] = isMealAdded.Error.Description;
    //        return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
    //    }

    //    TempData["Error"] = isMealAdded.Error.Description;
    //    return View(mealVM);
    //}

    //[HttpPost, ValidateAntiForgeryToken]
    //public IActionResult MealImage(IFormFile image)
    //{
    //    int mealId = int.Parse(_dataProtector.Unprotect(HttpContext!.Session.GetString(StaticData.MealId)!));

    //    Result changeMealImageResult = _mealService.ChangeMealImage(image, mealId);

    //    if (changeMealImageResult.IsSuccess)
    //    {
    //        TempData["Success"] = "Meal image Updated successfully";
    //        return RedirectToAction("MealDetails", new { id = _dataProtector.Protect(mealId.ToString()) });
    //    }

    //    if (changeMealImageResult.Error.Equals(MealErrors.MealNotFound))
    //    {
    //        TempData["Error"] = changeMealImageResult.Error.Description;
    //        return RedirectToAction("Index", "Home");
    //    }

    //    TempData["Error"] = changeMealImageResult.Error.Description;
    //    return RedirectToAction("MealDetails", new { id = _dataProtector.Protect(mealId.ToString()) });
    //}

    public IActionResult MealPriceHistoryDetails(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var mealPriceHistories = _mealPriceHistoryService.GetMealPriceHistories(mealId);

        return View(mealPriceHistories);
    }

    public IActionResult RestaurantDeletedMeals(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        HttpContext.Session.SetString(StaticData.RestaurantId, _dataProtector.Protect(restaurantId.ToString()));

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

        int restaurantId = (int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString(StaticData.RestaurantId)!)));

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

        int restaurantId = int.Parse(_dataProtector.Unprotect(HttpContext.Session.GetString(StaticData.RestaurantId)!));

        return RedirectToAction("RestaurantDeletedMeals", "RestaurantMeals", new { id = _dataProtector.Protect(restaurantId.ToString()   ) });
    }
}
