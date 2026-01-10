namespace Otlob.Core.Entities;

public sealed class AdminMonthlyAnalytic
{
    public int Id { get; set; }

    public int Year { get; set; } // Has Index
    public int Month { get; set; } // Has Index

    public int OrdersCount { get; set; }
    public decimal TotalOrdersSales { get; set; }
    
    public decimal TotalOrdersRevenue { get; } // Computed as percentage (5%) * TotalOrdersSales
}
