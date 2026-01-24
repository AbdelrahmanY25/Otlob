namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class SearchController(IApiSearchService apiSearchService) : ControllerBase
{
    private readonly IApiSearchService _apiSearchService = apiSearchService;

    [HttpGet]
    public IActionResult Search([FromQuery] string query)
    {
        var response = _apiSearchService.Search(query);

        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }

    [HttpGet("category")]
    public IActionResult SearchByCategory([FromQuery] string category)
    {
        var response = _apiSearchService.SearchByCategory(category);
        
        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }
}
