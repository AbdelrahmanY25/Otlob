namespace Otlob.IServices;

public interface IAllRestaurantsAnalyticsService
{
    AllRestaurantsAnalyticsDashboardResponse GetDashboardAnalytics(int topCount = 10);
    AllRestaurantsAnalyticsDashboardResponse GetDashboardAnalyticsByYear(int year, int topCount = 10);
    AllRestaurantsAnalyticsDashboardResponse GetDashboardAnalyticsByMonth(int year, int month, int topCount = 10);
    List<RestaurantAnalyticsItemResponse> GetTopRestaurantsBySales(int year, int? month = null, int topCount = 10);
    List<RestaurantAnalyticsItemResponse> GetTopRestaurantsByOrdersCount(int year, int? month = null, int topCount = 10);
}
