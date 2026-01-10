namespace Otlob.IServices;

public interface IRestaurantMonthlyAnalyticsService
{
    void Add(int restaurantId);
    void AddForAllActiveRestaurants();
    void Update(int restaurantId, decimal totalOrderPrice);
    RestaurantMonthlyAnalyticsResponse? GetCurrentMonthAnalytics(int restaurantId);
    RestaurantMonthlyAnalyticsResponse? GetByDate(int restaurantId, int year, int month);
    RestaurantMonthlyAnalyticsResponse? GetCurrentYearAnalytics(int restaurantId);
    RestaurantMonthlyAnalyticsResponse? GetByYearAnalytics(int restaurantId, int year);
    List<RestaurantMonthlyAnalyticsResponse> GetLastTwelveMonthsAnalytics(int restaurantId);
}
