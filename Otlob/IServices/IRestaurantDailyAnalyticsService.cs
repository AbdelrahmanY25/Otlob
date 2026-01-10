namespace Otlob.IServices;

public interface IRestaurantDailyAnalyticsService
{
    void Add(int restaurantId);
    void AddForAllActiveRestaurants();
    void InitialUpdate(int restaurantId);
    void UpdatePreparingOrders(int restaurantId);
    void UpdateShippedOrders(int restaurantId);
    void UpdateDeliveredOrders(int restaurantId, decimal orderTotalPrice);
    void UpdateCancelledOrders(int restaurantId);
    RestaurantDailyAnalyticsResponse? GetToDay(int restaurantId);
    RestaurantDailyAnalyticsResponse? GetByDate(int restaurantId, DateOnly date);
}
