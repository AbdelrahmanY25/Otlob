namespace Otlob.Core.Contracts.ViewModel;

public class OrdersOverLastTwelveMonthsVM
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
