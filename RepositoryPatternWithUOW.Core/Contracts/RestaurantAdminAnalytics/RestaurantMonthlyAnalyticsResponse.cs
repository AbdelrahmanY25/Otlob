namespace Otlob.Core.Contracts.RestaurantAdminAnalytics;

public class RestaurantMonthlyAnalyticsResponse
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int CancelledOrdersCount { get; init; }
    public int CompletedOrdersCount { get; init; }
    public decimal TotalOrdersSales { get; init; }
    public decimal TotalOrdersRevenue { get; init; }
}