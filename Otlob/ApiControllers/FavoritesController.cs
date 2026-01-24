namespace Otlob.ApiControllers;

[Route("api/[controller]")]
[ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FavoritesController(IFavouritesService favouritesService) : ControllerBase
{
    private readonly IFavouritesService _favouritesService = favouritesService;

    [HttpGet()]
    public IActionResult GetFavorites()
    {
        var favourites = _favouritesService.GetFavoritesList();
        return Ok(favourites);
    }

    [HttpPost("toggle")]
    public IActionResult Toggle([FromBody] ToggleToFavouritesRequest request)
    {
        if (string.IsNullOrEmpty(request.RestaurantKey))
            return BadRequest(new { success = false, message = "Invalid restaurant key" });

        var result = _favouritesService.Toggle(request.RestaurantKey);
        var isFavourite = _favouritesService.IsFavorite(request.RestaurantKey);

        return Ok(new { success = result, isFavourite });
    }

    [HttpGet("isFavorite")]
    public IActionResult IsFavorite([FromQuery] string restaurantKey)
    {
        if (string.IsNullOrEmpty(restaurantKey))
            return Ok(new { isFavourite = false });

        var isFavourite = _favouritesService.IsFavorite(restaurantKey);
        
        return Ok(new { isFavourite });
    }
}
