namespace Otlob.Core.Contracts.RestaurantAdminAnalytics;

public class RestaurantMonthlyAnalyticsResponse
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int OrdersCount { get; init; }
    public decimal TotalOrdersSales { get; init; }
    public decimal TotalOrdersRevenue { get; init; }
}