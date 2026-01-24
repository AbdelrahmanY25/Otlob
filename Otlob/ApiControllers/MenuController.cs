namespace Otlob.ApiControllers;

[Route("api/restaurant/[controller]")]
[EnableRateLimiting(RateLimiterPolicy.IpLimit)]
[ApiController]
public class MenuController(IApiMenuServcie apiMenuServcie) : ControllerBase
{
    private readonly IApiMenuServcie _apiMenuServcie = apiMenuServcie;

    [HttpGet("{restaurantKey}")]
    public IActionResult Menu(string restaurantKey)
    {
        var response = _apiMenuServcie.MenuForCustomer(restaurantKey);

        return Ok(response);
    }

    [HttpGet("meal/{mealId}")]
    public IActionResult Meal(string mealId)
    {
        var response = _apiMenuServcie.GetMeal(mealId);

        return Ok(response);
    }
}
