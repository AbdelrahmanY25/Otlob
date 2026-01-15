namespace Otlob.Core.Contracts.AdminAnalytics;

public class RestaurantAnalyticsItemResponse
{
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public string? RestaurantImage { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public int CancelledOrdersCount { get; init; }
    public int CompletedOrdersCount { get; init; }
    public decimal TotalOrdersSales { get; init; }
    public decimal TotalOrdersRevenue { get; init; }
}
