namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin)]
public class RestaurantStatusController(IRestauranStatusService restauranStatusService) : Controller
{
    private readonly IRestauranStatusService _restauranStatusService = restauranStatusService;

    public IActionResult ChangeRestaurantStatus(string id, AcctiveStatus status)
    {
        Result isStatusChanged = _restauranStatusService.ChangeRestauranStatus(id, status);

        TempData[isStatusChanged.IsSuccess ? "Success" : "Error"] =
            isStatusChanged.IsSuccess ? "The resturant status has been changed" : "Try again change status";

        return RedirectToAction("ResturantDetails", "Restaurants", new { id });
    }
}
