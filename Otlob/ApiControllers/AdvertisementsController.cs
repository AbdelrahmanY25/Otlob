namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class AdvertisementsController(IAdvertisementService advertisementService) : ControllerBase
{
    private readonly IAdvertisementService _advertisementService = advertisementService;

    /// <summary>
    /// Track a view for an advertisement
    /// </summary>
    [HttpPost("{id}/view")]
    public IActionResult TrackView(Guid id)
    {
        _advertisementService.TrackView(id);
        return Ok();
    }

    /// <summary>
    /// Track a click for an advertisement
    /// </summary>
    [HttpPost("{id}/click")]
    public IActionResult TrackClick(Guid id)
    {
        _advertisementService.TrackClick(id);
        return Ok();
    }

    /// <summary>
    /// Get active advertisements for display
    /// </summary>
    [HttpGet("active")]
    public IActionResult GetActiveAds()
    {
        var ads = _advertisementService.GetActiveAdsForHomePage();
        return Ok(ads);
    }
}
