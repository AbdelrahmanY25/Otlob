namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class HomeController(ICustomerSercice customerSercice, IAdvertisementService advertisementService) : Controller
{
    private readonly ICustomerSercice _customerSercice = customerSercice;
    private readonly IAdvertisementService _advertisementService = advertisementService;

    public IActionResult Home()
    {
        var activeAds = _advertisementService.GetActiveAdsForHomePage();
        return View("HomePage", activeAds);
    }

    [Authorize]
    public IActionResult Index()
    {        
        var ads = _advertisementService.GetActiveAdsForRestaurantsPage();
        var response = _customerSercice.GetCustomerHomePage(ads);

        if (response.IsFailure)
        {
            return response.Error.Equals(BranchErrors.NoRestaurantsAvailableInYourArea) ?
                View("NoBranchesAvaliable") :
                RedirectToAction("SavedAddresses", "Address", new { Area = DefaultRoles.Customer });
        }

        return View(response.Value);
    }
    
    public IActionResult Restaurants(double? lat = null, double? lon = null)
    {        
        var ads = _advertisementService.GetActiveAdsForRestaurantsPage();
        var response = _customerSercice.GetCustomerHomePage(lat, lon, ads);

        if (response.IsFailure)
            return View("NoBranchesAvaliable");

        return View(nameof(Index), response.Value);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
