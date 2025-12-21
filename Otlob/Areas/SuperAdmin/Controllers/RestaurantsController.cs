namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class RestaurantsController(IRestaurantService restaurantService) : Controller
{
    private readonly IRestaurantService _restaurantService = restaurantService;

    public IActionResult UnAcceptedRestaurants()
    {
        var response = _restaurantService.GetUnAcceptedAndPendingRestaurants();

        return View(response);
    }

    public IActionResult ActiveResturatns()
    {
        var response = _restaurantService.GetAcctiveRestaurants();

        return View(response);
    }

    public IActionResult ResturantDetails(string id)
    {        
        var result = _restaurantService.GetRestaurantDetailsById(id);

        if(result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        return View(result.Value);
    }

    public IActionResult DeletedRestaurant()
    {
        var restaurants = _restaurantService.GetDeletedRestaurants();

        if (restaurants is null)
        {
            TempData["Error"] = "There is no deleted restaurants";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        return View(restaurants);
    }

    public async Task<IActionResult> DeleteRestaurant(string id)
    {
        await _restaurantService.DelteRestaurant(id);
        TempData["Success"] = "Restaurant Deleted Successfully";
        return RedirectToAction(nameof(ActiveResturatns));
    }

    public async Task<IActionResult> UnDeleteRestaurant(string id)
    {
        await _restaurantService.UnDelteRestaurant(id);
        TempData["Success"] = "Restaurant UnDeleted Successfully";
        return RedirectToAction(nameof(ActiveResturatns));
    }
}
