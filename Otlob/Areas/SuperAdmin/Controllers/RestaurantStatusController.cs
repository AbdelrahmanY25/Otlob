namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole)]
public class RestaurantStatusController(IRestauranStatusService restauranStatusService) : Controller
{
    private readonly IRestauranStatusService _restauranStatusService = restauranStatusService;

    public IActionResult ChangeRestaurantStatus(string id, AcctiveStatus status)
    {
        Result isStatusChanged = _restauranStatusService.ChangeRestauranStatus(id, status);

        TempData[isStatusChanged.IsSuccess ? "Success" : "Error"] =
            isStatusChanged.IsSuccess ? "The resturant status has been changed" : "Try agin change status";

        return RedirectToAction("ActiveResturatns", "Restaurants", new { Area = SD.superAdminRole });
    }
}
