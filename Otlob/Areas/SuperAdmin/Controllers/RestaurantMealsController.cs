namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin")]
    public class RestaurantMealsController : Controller
    {
        private readonly IMealService mealService;
        private readonly IImageService imageService;
        private readonly IDataProtector dataProtector;
        private readonly IMealPriceHistoryService mealPriceHistoryService;

        public RestaurantMealsController(IMealService mealService,
                                        IImageService imageService,
                                         IDataProtectionProvider dataProtectionProvider,
                                         IMealPriceHistoryService mealPriceHistoryService)
        {
            this.mealService = mealService;
            this.imageService = imageService;
            this.mealPriceHistoryService = mealPriceHistoryService;
            this.dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IActionResult RestaurantMeals(string id)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));
            HttpContext.Session.SetString("restaurantId", dataProtector.Protect(restaurantId.ToString()));
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

            int restaurantId = int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!));

            var isMealAdded = mealService.AddMeal(mealVM, restaurantId, image);

            if (isMealAdded is string)
            {
                ModelState.AddModelError("", isMealAdded);
                return View(mealVM);
            }

            return RedirectToAction("RestaurantMeals", new { id = dataProtector.Protect(restaurantId.ToString()) });
        }

        public IActionResult MealDetails(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));

            MealVm mealVM = mealService.GetMealVM(mealId);

            HttpContext.Session.SetString("MealId", dataProtector.Protect(mealId.ToString()));

            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult MealDetails(MealVm mealVM)
        {
            if (!ModelState.IsValid)
            {
                return View(mealVM);
            }

            int mealId = int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("MealId")!));

            string isMealAdded = mealService.EditMeal(mealVM, mealId);

            if (isMealAdded is string)
            {
                TempData["Error"] = isMealAdded;
                return View(mealVM);
            }

            TempData["Success"] = "Meal info Updated successfully";
            return RedirectToAction("MealDetails", new { id = dataProtector.Protect(mealId.ToString()) });
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

        public IActionResult RestaurantDeletedMeals(string id)
        {
            int restaurantId = int.Parse(dataProtector.Unprotect(id));

            HttpContext.Session.SetString("restaurantId", dataProtector.Protect(restaurantId.ToString()));

            IQueryable<MealVm> mealsVM = mealService.GetDeletedMeals(restaurantId);

            return View(mealsVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteMeal(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));

            mealService.DeleteMeal(mealId);

            int restaurantId = (int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!)));

            return RedirectToAction("RestaurantMeals", "RestaurantMeals", new { id = dataProtector.Protect(restaurantId.ToString()) });
        }

        public IActionResult UnDeleteMeal(string id)
        {
            int mealId = int.Parse(dataProtector.Unprotect(id));

            mealService.UnDeleteMeal(mealId);

            int restaurantId = int.Parse(dataProtector.Unprotect(HttpContext.Session.GetString("restaurantId")!));

            return RedirectToAction("RestaurantDeletedMeals", "RestaurantMeals", new { id = dataProtector.Protect(restaurantId.ToString()   ) });
        }
    }
}
