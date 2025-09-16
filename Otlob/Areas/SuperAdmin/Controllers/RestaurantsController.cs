namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(SD.superAdminRole), Authorize(Roles = SD.superAdminRole)]
public class RestaurantsController(IRestaurantService restaurantService, IDataProtectionProvider dataProtectionProvider) : Controller
{
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public IActionResult ResturatnRequist()
    {
        var resturantsVM = _restaurantService.GetAllRestaurants(filter: null, statuses: [AcctiveStatus.UnAccepted]);

        if (resturantsVM is null)
        {
            TempData["Error"] = "There is no resturant requist";
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole } );
        }

        return View(resturantsVM);
    }

    public IActionResult ActiveResturatns()
    {
        AcctiveStatus[] acceptedStatuses = [AcctiveStatus.Acctive, AcctiveStatus.Warning, AcctiveStatus.Block];

        var resturantsVM = _restaurantService.GetAllRestaurants(filter: null, statuses: acceptedStatuses);

        if (resturantsVM is null)
        {
            TempData["Error"] = "There is no resturant requist";
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        return View(resturantsVM);
    }

    public IActionResult ResturantDetails(string id)
    {
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        var result = _restaurantService.GetRestaurantDetailsById(restaurantId);

        if(result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
        }

        return View(result.Value);
    }

    public IActionResult DeletedRestaurant()
    {
        var restaurants = _restaurantService.GetDeletedRestaurants();

        if (restaurants is null)
        {
            TempData["Error"] = "There is no deleted restaurants";
            return RedirectToAction("Index", "Home", new { Area = SD.superAdminRole });
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
