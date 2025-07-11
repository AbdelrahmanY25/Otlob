namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class MealController : Controller
    {
        private readonly IMealService mealService;
        private readonly IImageService imageService;
        private readonly IDataProtector dataProtector;
        private readonly IMealPriceHistoryService mealPriceHistoryService;

        public MealController(IMealService mealService,
                              IImageService imageService,
                              IDataProtectionProvider dataProtectionProvider,
                              IMealPriceHistoryService mealPriceHistoryService)
        {
            this.mealService = mealService;
            this.imageService = imageService;
            this.mealPriceHistoryService = mealPriceHistoryService;
            this.dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);
            var mealsVM = mealService.ViewMealsVmToRestaurantAdminSummary(restaurantId);

            return View(mealsVM);
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

            var isMealadded = mealService.AddMeal(mealVM, restaurantId, image);

            if (isMealadded is string)
            {
                ModelState.AddModelError("", isMealadded);
                return View(mealVM);
            }

            return BackToMealsView("Your New Meal Added Successfully");                            
        }

        public IActionResult MealDetails(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));

            var mealVM = mealService.GetMealVM(mealId);

            HttpContext.Session.SetString("MealId", dataProtector.Protect(mealId.ToString()));

            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult MealDetails(MealVm mealVM)
        {
            int mealId = int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("MealId")!));

            if (!ModelState.IsValid)
            {
                return View(mealVM);
            }

            string isMealAdded = mealService.EditMeal(mealVM, mealId);

            if (isMealAdded is string)
            {
                TempData["Error"] = isMealAdded;
                return View(mealVM);
            }

            return BackToMealsView("Your Old Meal Updated Successfully");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult MealImage(IFormFile image)
        {
            int mealId = int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("MealId")!));

            var isImageUpdated = imageService.UploadImage(image);

            if (!isImageUpdated.IsSuccess)
            {
                TempData["Error"] = isImageUpdated.Message;
                return RedirectToAction("MealDetails", new { id = dataProtector.Protect(mealId.ToString()) });
            }

            Meal meal = mealService.GetMealImageById(mealId);
            
            var isOldImageDeleted = imageService.DeleteOldImageIfExist(meal.Image);

            if (!isOldImageDeleted.IsSuccess)
            {
                TempData["Error"] = isOldImageDeleted.Message;
                return RedirectToAction("MealDetails", new { id = dataProtector.Protect(mealId.ToString()) });
            }

            mealService.UpdateMealImage(meal, isImageUpdated.ImageUrl);

            TempData["Success"] = "Meal image Updated successfully";
            return RedirectToAction("MealDetails", new { id = dataProtector.Protect(mealId.ToString()) });
        }

        public IActionResult MealPriceHistoryDetails(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));

            var mealPriceHistories = mealPriceHistoryService.GetMealPriceHistories(mealId);

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

            IQueryable<MealVm> mealsVM = mealService.GetDeletedMeals(restaurantId);

            if (mealsVM.IsNullOrEmpty())
            {
                TempData["Error"] = "There is no deleted meals";
                return RedirectToAction("Index", "Home");
            }

            return View(mealsVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteMeal(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));
            mealService.DeleteMeal(mealId);
            return RedirectToAction("Index");
        }

        public IActionResult UnDeleteMeal(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));
            mealService.UnDeleteMeal(mealId);

            return RedirectToAction("DeletedMeals");
        }

        private IActionResult BackToMealsView(string msg)
        {
            TempData["Success"] = msg;
            return RedirectToAction("Index");
        }
    }
}
