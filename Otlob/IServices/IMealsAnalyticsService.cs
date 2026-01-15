namespace Otlob.IServices;

public interface IMealsAnalyticsService
{
    void UpdateSales(int restaurantId, string mealId, int quantity, decimal saleAmount);
    IEnumerable<MealsAnalyticsResponse> GetAllByRestaurantId(int restaurantId);
    IEnumerable<MealsAnalyticsResponse> GetTopTenSales(int restaurantId);
}
