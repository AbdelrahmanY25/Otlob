namespace Otlob.Areas.ResturantAdmin.Controllers;

[Area(SD.restaurantAdmin)]
[Authorize(Roles = SD.restaurantAdmin)]
public class HomeController(IRestaurantService restaurantService) : Controller
{
    private readonly IRestaurantService restaurantService = restaurantService;

    public IActionResult Index()
    {        
        int restaurantId = int.Parse(User.FindFirstValue(SD.restaurantId)!);       

        var result = restaurantService.GetRestaurant(restaurantId);
      
        if (result.IsFailure)
        {
            return RedirectToAction("Login", "Account", new { Area = SD.customer });
        }

        return View(result.Value);
    }
}
