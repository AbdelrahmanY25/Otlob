namespace Otlob.IServices;

public interface IRestaurantRatingAnlyticsService
{
    void Add(int restaurantId);
    void UpdateRate(int restaurantId, decimal rate);
    RestaurantRatingAnlyticsResponse GetByRestaurantId(int restaurantId);
    IEnumerable<RestaurantRatingAnlyticsResponse> GetAll();
}