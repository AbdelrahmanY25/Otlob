namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin)]
[Authorize(Roles = DefaultRoles.RestaurantAdmin), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class HomeController(IRestaurantService restaurantService,
                            IRestaurantDailyAnalyticsService restaurantDailyAnalyticsService,
                            IRestaurantMonthlyAnalyticsService restaurantMonthlyAnalyticsService) : Controller
{
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IRestaurantDailyAnalyticsService _restaurantDailyAnalyticsService = restaurantDailyAnalyticsService;
    private readonly IRestaurantMonthlyAnalyticsService _restaurantMonthlyAnalyticsService = restaurantMonthlyAnalyticsService;

    public IActionResult Index()
    {
        AcctiveStatus status = _restaurantService
            .GetRestaurantStatusById(int.Parse(User.FindFirstValue(StaticData.RestaurantId)!));

        switch (status)
        {
            case AcctiveStatus.UnAccepted:
                return RedirectToAction(nameof(UnAcceptedRestaurant));

            case AcctiveStatus.Pending:
                return RedirectToAction(nameof(PendingRestaurant));

            case AcctiveStatus.Acctive:
                return RedirectToAction(nameof(AcctiveRestaurants));

            case AcctiveStatus.Warning:
                return RedirectToAction(nameof(AcctiveRestaurants));

            case AcctiveStatus.Block:
                return RedirectToAction(nameof(AcctiveRestaurants));

            default:
                break;
        }

        return View();
    }

    public IActionResult UnAcceptedRestaurant() => 
        View(_restaurantService.GetUnAcceptedRestaurant());
    
    
    public IActionResult PendingRestaurant() => 
        View(_restaurantService.GetPendingRestaurant());
    
    public IActionResult AcctiveRestaurants()
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        var response = new RestaurantAdminDashboardResponse
        {
            TodayAnalytics = _restaurantDailyAnalyticsService.GetToDay(restaurantId),
            CurrentMonthAnalytics = _restaurantMonthlyAnalyticsService.GetCurrentMonthAnalytics(restaurantId),
            CurrentYearAnalytics = _restaurantMonthlyAnalyticsService.GetCurrentYearAnalytics(restaurantId),
            Last12MonthsAnalytics = _restaurantMonthlyAnalyticsService.GetLastTwelveMonthsAnalytics(restaurantId)
        };

        return View(response);
    }

    [HttpGet]
    public IActionResult GetDailyAnalytics(string date)
    {
        int restaurantId = int.Parse(User.FindFirstValue(StaticData.RestaurantId)!);

        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest("Invalid date format");

        var analytics = _restaurantDailyAnalyticsService.GetByDate(restaurantId, parsedDate);
        
        if (analytics is null)
            return NotFound("No data for this date");

        return Json(analytics);
    }
}
