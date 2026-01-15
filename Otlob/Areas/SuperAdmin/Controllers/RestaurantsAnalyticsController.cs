namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class RestaurantsAnalyticsController(IAllRestaurantsAnalyticsService allRestaurantsAnalyticsService,
                                            IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
                                            IRestaurantMonthlyAnalyticsService restaurantMonthlyAnalyticsService,
                                            IMealsAnalyticsService mealsAnalyticsService,
                                            IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IMealsAnalyticsService _mealsAnalyticsService = mealsAnalyticsService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IAllRestaurantsAnalyticsService _allRestaurantsAnalyticsService = allRestaurantsAnalyticsService;
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;
    private readonly IRestaurantMonthlyAnalyticsService _restaurantMonthlyAnalyticsService = restaurantMonthlyAnalyticsService;

    public IActionResult Index()
    {
        var response = _allRestaurantsAnalyticsService.GetDashboardAnalytics();
        return View(response);
    }

    public IActionResult Main(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var response = new RestaurantAdminDashboardResponse
        {
            TodayAnalytics = _restaurantDailyAnalyticsService.GetToDay(restaurantId),
            CurrentMonthAnalytics = _restaurantMonthlyAnalyticsService.GetCurrentMonthAnalytics(restaurantId),
            CurrentYearAnalytics = _restaurantMonthlyAnalyticsService.GetCurrentYearAnalytics(restaurantId),
            Last12MonthsAnalytics = _restaurantMonthlyAnalyticsService.GetLastTwelveMonthsAnalytics(restaurantId),
            TopTenMealsSales = _mealsAnalyticsService.GetTopTenSales(restaurantId).ToList()
        };

        ViewBag.RestaurantKey = restaurantKey;

        return View(response);
    }
    
    public IActionResult GeneralAnalytics(string restaurantKey)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));

        var response = _restaurantMonthlyAnalyticsService.GetGeneralAnalytics(restaurantId);

        return View(response);
    }

    [HttpGet]
    public IActionResult GetDailyAnalytics(string date, string restaurantKey)
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest("Invalid date format");

        var analytics = _restaurantDailyAnalyticsService.GetByDate(restaurantId, parsedDate);

        if (analytics is null)
            return NotFound("No data for this date");

        return Json(analytics);
    }

    [HttpGet]
    public IActionResult GetAnalyticsByYear(int year)
    {
        var response = _allRestaurantsAnalyticsService.GetDashboardAnalyticsByYear(year);
        return Json(response);
    }

    [HttpGet]
    public IActionResult GetAnalyticsByMonth(int year, int month)
    {
        var response = _allRestaurantsAnalyticsService.GetDashboardAnalyticsByMonth(year, month);
        return Json(response);
    }

    [HttpGet]
    public IActionResult GetTopRestaurantsBySales(int year, int? month = null, int topCount = 10)
    {
        var response = _allRestaurantsAnalyticsService.GetTopRestaurantsBySales(year, month, topCount);
        return Json(response);
    }

    [HttpGet]
    public IActionResult GetTopRestaurantsByOrders(int year, int? month = null, int topCount = 10)
    {
        var response = _allRestaurantsAnalyticsService.GetTopRestaurantsByOrdersCount(year, month, topCount);
        return Json(response);
    }
}
