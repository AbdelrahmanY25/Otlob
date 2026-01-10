namespace Otlob.Core.Entities;

public sealed class RestaurantDailyAnalytic
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public DateOnly Date { get; set; } // Has Index

    public int PendingOrders { get; set; }
    public int PreparingOrders { get; set; }
    public int ShippingOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int CompletedOrdersCount { get; set; }
    public decimal TotalOrdersSales { get; set; }

    public decimal TotalOrdersRevenue { get; }
    public decimal AverageOrderPrice { get; }

    // Navigation
    public Restaurant Restaurant { get; set; } = default!;
}
