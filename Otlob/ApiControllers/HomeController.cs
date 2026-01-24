namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController(IApiCustomerService apiCustomerService, IAdvertisementService advertisementService) : ControllerBase
{
    private readonly IApiCustomerService _apiCustomerService = apiCustomerService;
    private readonly IAdvertisementService _advertisementService = advertisementService;

    [HttpGet("restaurants"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetCustomerHomePage([FromQuery] double? lat = null, [FromQuery] double? lon = null)
    {
        var activeAds = _advertisementService.GetActiveAdsForRestaurantsPage();
        var response = _apiCustomerService.GetCustomerHomePage(lat, lon, activeAds);

        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }

    [HttpGet("restaurants/by-location")]
    public IActionResult GetCustomerHomePageByLocation([FromQuery] double? lat = null, [FromQuery] double? lon = null)
    {
        var activeAds = _advertisementService.GetActiveAdsForRestaurantsPage();
        var response = _apiCustomerService.GetCustomerHomePage(lat, lon, activeAds);
        
        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }
}
