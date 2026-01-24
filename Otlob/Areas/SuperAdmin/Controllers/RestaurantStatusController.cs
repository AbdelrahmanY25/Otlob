namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin)]
public class RestaurantStatusController(IRestauranStatusService restauranStatusService) : Controller
{
    private readonly IRestauranStatusService _restauranStatusService = restauranStatusService;

    public IActionResult ChangeRestaurantStatus(string id, AcctiveStatus status)
    {
        Result isStatusChanged = _restauranStatusService.ChangeRestauranStatus(id, status);

        return RedirectToAction("Details", "Restaurants", new { id });
    }
}
