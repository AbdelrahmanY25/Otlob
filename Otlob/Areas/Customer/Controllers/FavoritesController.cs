namespace Otlob.Areas.Customer.Controllers;

[Area(DefaultRoles.Customer), Authorize, EnableRateLimiting(RateLimiterPolicy.IpLimit)]
public class FavoritesController(IFavouritesService favouritesService) : Controller
{
    private readonly IFavouritesService _favouritesService = favouritesService;

    public IActionResult Index()
    {
        var favourites = _favouritesService.GetFavoritesList();
        return View(favourites);
    }

    [HttpPost]
    public IActionResult Toggle([FromBody] ToggleToFavouritesRequest request)
    {
        if (string.IsNullOrEmpty(request.RestaurantKey))
            return Json(new { success = false, message = "Invalid restaurant key" });

        var result = _favouritesService.Toggle(request.RestaurantKey);
        var isFavourite = _favouritesService.IsFavorite(request.RestaurantKey);

        return Json(new { success = result, isFavourite });
    }

    [HttpGet]
    public IActionResult IsFavorites(string restaurantKey)
    {
        if (string.IsNullOrEmpty(restaurantKey))
            return Json(new { isFavourite = false });

        var isFavourite = _favouritesService.IsFavorite(restaurantKey);
        
        return Json(new { isFavourite });
    }
}
