namespace Otlob.Core.Contracts.RestaurantAdminAnalytics;

public class RestaurantGeneralAnalyticsResponse
{
    public int TotalOrdersCount { get; init; }
    public decimal AverageOrdersPerDay { get; init; }
    public int TotalCancelledOrdersCount { get; init; }
    public decimal TotalSales { get; init; }
    public decimal TotalRevenue { get; init; }
    public List<RestaurantMonthlyAnalyticsResponse> AllMonthsAnalytics { get; init; } = [];
}
