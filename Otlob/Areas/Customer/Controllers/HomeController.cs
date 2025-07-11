namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IRestaurantService restaurantService;
        private readonly IMealService mealService;
        private readonly IDataProtector dataProtector;

        public HomeController(ILogger<HomeController> logger,
                              IRestaurantService restaurantService,
                              IMealService mealService,
                              IDataProtectionProvider dataProtectionProvider)
        {
            this.logger = logger;
            this.restaurantService = restaurantService;
            this.mealService = mealService;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
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

            int restaurantlId = int.Parse(dataProtector.Unprotect(id!));

            var meals = mealService.ViewAllMealsVm(restaurantlId);

            if (meals is null)
            {
                return NotFound();
            }

            if (filter is not null)
            {
                meals = mealService.MealCategoryFilter(meals, filter);
            }

            ViewBag.ResId = id;

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
