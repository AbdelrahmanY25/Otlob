namespace Otlob.IServices;

public interface IFavouritesService
{
    bool Toggle(string restaurantKey);
    IEnumerable<FavouritesResponse> GetFavoritesList();
    bool IsFavorite(string restaurantKey);
}
