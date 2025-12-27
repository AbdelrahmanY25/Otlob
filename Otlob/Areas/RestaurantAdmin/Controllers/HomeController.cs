namespace Otlob.Areas.RestaurantAdmin.Controllers;

[Area(DefaultRoles.RestaurantAdmin)]
[Authorize(Roles = DefaultRoles.RestaurantAdmin), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class HomeController(IRestaurantService restaurantService) : Controller
{
    private readonly IRestaurantService restaurantService = restaurantService;

    public IActionResult Index()
    {
        AcctiveStatus status = restaurantService
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
        View(restaurantService.GetUnAcceptedRestaurant());
    
    
    public IActionResult PendingRestaurant() => 
        View(restaurantService.GetPendingRestaurant());
    
    public IActionResult AcctiveRestaurants()
    {
        return View();
    }

    //TODO:
    // Actions for Warning and Blocked restaurants can be added here
}
