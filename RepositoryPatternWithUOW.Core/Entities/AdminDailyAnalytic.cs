namespace Otlob.Core.Entities;

public sealed class AdminDailyAnalytic
{
    public int Id { get; set; }

    public DateOnly Date { get; set; } // Has Index

    public int PendingOrders { get; set; }
    public int PreparingOrders { get; set; }
    public int ShippingOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int CompletedOrdersCount { get; set; }
    public decimal TotalOrdersSales { get; set; }

    public decimal TotalOrdersRevenue { get; } // Computed as percentage (5%) * TotalOrdersSales
    public decimal AverageOrderPrice { get; } // Computed as TotalOrdersSales / CompletedOrdersCount
}
