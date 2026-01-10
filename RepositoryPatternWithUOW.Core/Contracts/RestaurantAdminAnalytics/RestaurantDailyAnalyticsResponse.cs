namespace Otlob.Core.Contracts.RestaurantAdminAnalytics;

public class RestaurantDailyAnalyticsResponse
{
    public DateOnly Date { get; init; }
    public int PendingOrders { get; init; }
    public int PreparingOrders { get; init; }
    public int ShippingOrders { get; init; }
    public int DeliveredOrders { get; init; }
    public int CancelledOrders { get; init; }
    public int CompletedOrdersCount { get; init; }
    public decimal TotalOrdersSales { get; init; }
    public decimal TotalOrdersRevenue { get; init; }
    public decimal AverageOrderPrice { get; init; }
}