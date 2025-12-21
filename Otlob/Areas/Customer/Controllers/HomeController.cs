namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class HomeController(ILogger<HomeController> logger, IRestaurantService restaurantService,
                      IMealService mealService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IMealService _mealService = mealService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Home() => View("HomePage");

    public IActionResult Index(Category? filter = null)
    {
        var restaurants = _restaurantService.GetAcctiveRestaurants();

        return View(restaurants);
    }

    //public IActionResult Details(string id, string? filter = null)
    //{      
    //    var result = _mealService.GetMealsDetails(id);

    //    if (result.IsFailure)
    //    {
    //        return RedirectToAction("Index");
    //    }

    //    //if (filter is not null)
    //    //{
    //    //    meals = _mealService.MealCategoryFilter(result.Value, filter);
    //    //}

    //    ViewBag.ResId = id;

    //    return View(result.Value);
    //}
  
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
