namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class RestaurantsController(IRestaurantService restaurantService,
                                   ICommercialRegistrationService commercialRegistrationService,
                                   ITradeMarkService tradeMarkService,
                                   IVatService vatService,
                                   IBankAccountService bankAccountService,
                                   INationalIdService nationalIdService) : Controller
{
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly ICommercialRegistrationService _commercialRegistrationService = commercialRegistrationService;
    private readonly ITradeMarkService _tradeMarkService = tradeMarkService;
    private readonly IVatService _vatService = vatService;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly INationalIdService _nationalIdService = nationalIdService;

    public IActionResult UnAccepted()
    {
        var response = _restaurantService.GetUnAcceptedAndPendingRestaurants();

        return View(response);
    }

    public IActionResult Active()
    {
        var response = _restaurantService.GetAcctiveRestaurants();

        return View(response);
    }

    public IActionResult Details(string id, bool isAvaliable = true)
    {        
        var result = _restaurantService.GetRestaurantDetailsById(id, isAvaliable);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        var response = new RestaurantFullDetailsResponse
        {
            Restaurant = result.Value,
            CommercialRegistration = _commercialRegistrationService.GetCommercialRegistration(id).Value,
            TradeMark = _tradeMarkService.GetTradeMark(id).Value,
            Vat = _vatService.GetVat(id).Value,
            BankAccount = _bankAccountService.GetBankAccount(id).Value,
            NationalId = _nationalIdService.GetNationalId(id).Value
        };

        return View(response);
    }

    public IActionResult Deleted()
    {
        var restaurants = _restaurantService.GetDeletedRestaurants();

        if (restaurants is null)
        {
            TempData["Error"] = "There is no deleted restaurants";
            return RedirectToAction("Index", "Home", new { Area = DefaultRoles.SuperAdmin });
        }

        return View(restaurants);
    }

    public IActionResult Delete(string restaurantKey)
    {
        var result = _restaurantService.DelteRestaurant(restaurantKey);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Details));
        }

        TempData["Success"] = "Restaurant Deleted Successfully";
        
        return RedirectToAction(nameof(Deleted));
    }

    public IActionResult Restore(string restaurantKey)
    {
        var result = _restaurantService.UnDelteRestaurant(restaurantKey);
        
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Description;
            return RedirectToAction(nameof(Deleted));
        }


        TempData["Success"] = "Restaurant UnDeleted Successfully";        
        return RedirectToAction(nameof(Active));
    }
}
