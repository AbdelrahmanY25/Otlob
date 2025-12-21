namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class MealsController(IMealService mealService, IMealCategoryService menuCategoryService, 
                             IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealService _mealService = mealService;
    private readonly IMealCategoryService _menuCategoryService = menuCategoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    //public IActionResult Index(string id)
    //{                
    //    var result = _mealService.GetAllByRestaurantId(int.Parse(_dataProtector.Unprotect(id)));
        
    //    if (result!.IsFailure)
    //    {
    //        TempData["Error"] = result.Error.Description;
    //        return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
    //    }
        
    //    HttpContext.Session.SetString(StaticData.RestaurantId, id);

    //    return View(result.Value);
    //}

    public IActionResult Add()
    {
        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey)) 
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

                                                                        // TODO: Handle Exception
        var categoriesResult = _menuCategoryService.GetAllByRestaurantId(int.Parse(_dataProtector.Unprotect(restaurantKey)));

        if (categoriesResult!.IsFailure)
        {
            TempData["Error"] = categoriesResult.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        MealRequest request = new() { Categories = categoriesResult.Value };
        ViewBag.ResKey = restaurantKey;
        
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

        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

                                                         // TODO: Handle Exception
        Result isMealAdded = await _mealService.AddAsync(int.Parse(_dataProtector.Unprotect(restaurantKey)), request, imageRequest);

        if (isMealAdded.IsFailure)
        {
            ModelState.AddModelError("", isMealAdded.Error.Description);
            return View(request);
        }

        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }

    public IActionResult Update(string key)
    {
        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        var result = _mealService.GetForUpdate(key);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }
        
        ViewBag.ResKey = restaurantKey;
        
        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(MealRequest request, string key)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Update), new { key });
        }

        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

                                                                           // TODO: Handle Exception
        Result isMealAdded = await _mealService.UpdateAsync(request, key, int.Parse(_dataProtector.Unprotect(restaurantKey)));

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

    public IActionResult Deleted(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        var result = _mealService.GetDeletedMeals(restaurantId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home");
        }

        HttpContext.Session.SetString(StaticData.RestaurantId, id);
        
        return View(result.Value);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(string id)
    {

        string restaurantKey = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(restaurantKey))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }
        
        var result = _mealService.DeleteMeal(id);

        if (result.IsFailure)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction("Menu", "Menu", new { restaurantKey });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UnDelete(string id)
    {
        string resId = HttpContext.Session.GetString(StaticData.RestaurantId)!;

        if (string.IsNullOrEmpty(resId))
        {
            TempData["Error"] = "The session timeout try again";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.RestaurantAdmin });
        }

        var result = _mealService.UnDeleteMeal(id);

        if (result.IsFailure)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction(nameof(Deleted), new { id = resId });
    }
}
