namespace Otlob.IServices;

public interface IRestaurantBusinessDetailsService
{
    Result<RestaurantBusinessInfo> GetByRestaurantId(int restaurantId);
    Result Update(RestaurantBusinessInfo request, int restaurantId);
}