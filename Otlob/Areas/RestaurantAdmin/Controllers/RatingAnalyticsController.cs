namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin)]
[Authorize(Roles = DefaultRoles.RestaurantAdmin)]
public class RatingAnalyticsController(IRestaurantRatingAnlyticsService ratingAnalyticsService, IHttpContextAccessor httpContextAccessor) : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IRestaurantRatingAnlyticsService _ratingAnalyticsService = ratingAnalyticsService;

    public IActionResult Index()
    {
        var restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);
        
        var analytics = _ratingAnalyticsService.GetByRestaurantId(restaurantId);
        
        return View(analytics);
    }
}
