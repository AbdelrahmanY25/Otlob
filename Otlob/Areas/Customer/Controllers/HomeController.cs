namespace Otlob.Areas.Customer.Controllers;

[Area(SD.customer)]
public class HomeController(ILogger<HomeController> logger, IRestaurantService restaurantService,
                      IMealService mealService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IMealService _mealService = mealService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult Index(Category? filter = null)
    {
        var restaurants = _restaurantService.GetAllRestaurants(filter);

        if (restaurants is null)
        {
            return View("NotFound");
        }

        return View(restaurants);
    }

    public IActionResult Details(string? id, string? filter = null)
    {
        if (id.IsNullOrEmpty())
        {
            return RedirectToAction("Index");
        }

        int restaurantlId = int.Parse(_dataProtector.Unprotect(id!));

        var result = _mealService.GetMealsDetails(restaurantlId);
        var meals = result.Value;


        if (result.IsFailure)
        {
            return RedirectToAction("Index");
        }

        if (filter is not null)
        {
            meals = _mealService.MealCategoryFilter(result.Value, filter);
        }

        ViewBag.ResId = id;

        return View(meals);
    }
  
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
