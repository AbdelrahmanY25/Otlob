namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IEncryptionService encryptionService;
        private readonly IRestaurantService restaurantService;
        private readonly IMealService mealService;

        public HomeController(ILogger<HomeController> logger,
                              IEncryptionService encryptionService,
                              IRestaurantService restaurantService,
                              IMealService mealService)
        {
            this.logger = logger;
            this.encryptionService = encryptionService;
            this.restaurantService = restaurantService;
            this.mealService = mealService;
        }

        public IActionResult Index(RestaurantCategory? filter = null)
        {
            var restaurants = restaurantService.GetAllRestaurantsJustMainInfo(filter);
           
            return View(restaurants);
        }

        public IActionResult Details(string? id, string? filter = null)
        {
            if (id.IsNullOrEmpty())
            {
                return RedirectToAction("Index");
            }

            int restaurantlId = encryptionService.DecryptId(id);

            var meals = mealService.ViewAllMealsVm(restaurantlId);

            if (meals is null)
            {
                return NotFound();
            }

            if (filter is not null)
            {
                meals = mealService.MealCategoryFilter(meals, filter);
            }

            ViewBag.ResId = encryptionService.EncryptId(restaurantlId);

            return View(meals);
        }
      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
