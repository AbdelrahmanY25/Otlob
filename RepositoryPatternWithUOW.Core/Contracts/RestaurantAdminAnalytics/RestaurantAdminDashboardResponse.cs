namespace Otlob.Core.Contracts.RestaurantAdminAnalytics;

public class RestaurantAdminDashboardResponse
{
    public RestaurantDailyAnalyticsResponse? TodayAnalytics { get; set; }
    public RestaurantMonthlyAnalyticsResponse? CurrentMonthAnalytics { get; set; }
    public RestaurantMonthlyAnalyticsResponse? CurrentYearAnalytics { get; set; }
    public List<RestaurantMonthlyAnalyticsResponse> Last12MonthsAnalytics { get; set; } = [];
}
