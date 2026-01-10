namespace Otlob.Core.Entities;

public sealed class RestaurantMonthlyAnalytic
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public int Year { get; set; } // Has Index
    public int Month { get; set; } // Has Index

    public int OrdersCount { get; set; }
    public decimal TotalOrdersSales { get; set; }
    
    public decimal TotalOrdersRevenue { get; }

    public Restaurant Restaurant { get; set; } = default!;
}
