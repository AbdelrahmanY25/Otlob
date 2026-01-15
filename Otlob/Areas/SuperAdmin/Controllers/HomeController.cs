namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class HomeController(IAdminDailyAnalyticsService adminDailyAnalyticsService,
                            IAdminMonthlyAnalyticsService adminMonthlyAnalyticsService) : Controller
{
    private readonly IAdminDailyAnalyticsService _adminDailyAnalyticsService = adminDailyAnalyticsService;
    private readonly IAdminMonthlyAnalyticsService _adminMonthlyAnalyticsService = adminMonthlyAnalyticsService;

    public IActionResult Index()
    {
        var response = new SuperAdminDashboardResponse
        {
            TodayAnalytics = _adminDailyAnalyticsService.GetToDay(),
            CurrentMonthAnalytics = _adminMonthlyAnalyticsService.GetCurrentMonthAnalytics(),
            CurrentYearAnalytics = _adminMonthlyAnalyticsService.GetCurrentYearAnalytics(),
            Last12MonthsAnalytics = _adminMonthlyAnalyticsService.GetLastTweleveMonthsAnalytics()
        };

        return View(response);
    }

    public IActionResult GeneralAnalytics()
    {
        var response = _adminMonthlyAnalyticsService.GetGeneralAnalytics();
        return View(response);
    }

    [HttpGet]
    public IActionResult GetDailyAnalytics(string date)
    {
        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest("Invalid date format");

        var analytics = _adminDailyAnalyticsService.GetByDate(parsedDate);
        
        if (analytics is null)
            return NotFound("No data for this date");

        return Json(analytics);
    }
}
