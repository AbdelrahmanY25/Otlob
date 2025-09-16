using Otlob.Abstractions;

namespace Otlob.Areas.ResturantAdmin.Controllers;

[Area(SD.restaurantAdmin)]
public class MealController(IMealService mealService, IImageService imageService,
                      IDataProtectionProvider dataProtectionProvider, IMealPriceHistoryService mealPriceHistoryService, IAuthService authService) : Controller
{
    private readonly IMealService _mealService = mealService;
    private readonly IImageService _imageService = imageService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;
    private readonly IAuthService _authService = authService;

    public IActionResult Index()
    {
        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);
        
        var result = _mealService.GetMealsByRestaurantId(restaurantId);

        if (result.IsFailure)
        {
            _authService.LogOutAsync();
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

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return RedirectToAction("Login", "Account", new { Area = "Customer" });
        }

        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

        Result isMealadded = _mealService.AddMeal(mealVM, restaurantId, image);

        if (isMealadded.IsFailure)
        {
            ModelState.AddModelError("", isMealadded.Error.Description);
            return View(mealVM);
        }

        return BackToMealsView("Meal Added Succefully");
    }

    public IActionResult MealDetails(string id)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(id));

        var result = _mealService.GetMealVM(mealId);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = SD.restaurantAdmin });
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

        if (isMealAdded.IsFailure)
        {
            TempData["Error"] = isMealAdded.Error.Description;
            return View(mealVM);
        }

        return BackToMealsView("Meal updated succefully");
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

    public IActionResult DeletedMeals()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return RedirectToAction("Login", "Account", new { Area = "Customer" });
        }

        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);

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
        return RedirectToAction("Index");
    }
}
