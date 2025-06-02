namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin")]
    public class MealController : Controller
    {
        private readonly IMealService mealService;
        private readonly IEncryptionService encryptionService;
        private readonly IMealPriceHistoryService mealPriceHistoryService;

        public MealController(IMealService mealService,
                             IEncryptionService encryptionService,
                             IMealPriceHistoryService mealPriceHistoryService)
        {
            this.mealService = mealService;
            this.encryptionService = encryptionService;
            this.mealPriceHistoryService = mealPriceHistoryService;
        }

        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId));
            var mealsVM = mealService.ViewMealsVmToRestaurantAdminSummary(restaurantId);

            return View(mealsVM);
        }

        public IActionResult AddMeal() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(MealVm mealVM)
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

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId));

            var isMealadded = await mealService.AddMeal(mealVM, restaurantId, Request.Form.Files);

            if (isMealadded is string)
            {
                ModelState.AddModelError("", isMealadded);
                return View(mealVM);
            }

            return BackToMealsView("Your New Meal Added Successfully");                            
        }

        public IActionResult MealDetails(string id)
        {
            var mealId = encryptionService.DecryptId(id);

            var mealVM = mealService.GetMealVM(mealId);

            HttpContext.Session.SetString("MealId", encryptionService.EncryptId(mealId));

            return View(mealVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MealDetails(MealVm mealVM)
        {
            int mealId = encryptionService.DecryptId(HttpContext.Session.GetString("MealId"));

            if (!ModelState.IsValid)
            {
                return View(mealVM);
            }

            string isMealAdded = await mealService.EditMeal(mealVM, mealId, Request.Form.Files);

            if (isMealAdded is string)
            {
                ModelState.AddModelError("", isMealAdded);
                return View(mealVM);
            }

            return BackToMealsView("Your Old Meal Updated Successfully");        
        }

        public IActionResult MealPriceHistoryDetails(string id)
        {
            int mealId = encryptionService.DecryptId(id);

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

            int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId));

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
            int mealId = encryptionService.DecryptId(id);
            mealService.DeleteMeal(mealId);
            return RedirectToAction("Index");
        }

        public IActionResult UnDeleteMeal(string id)
        {
            int mealId = encryptionService.DecryptId(id);
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
