namespace Otlob.Core.Contracts.Authentication;

public class OrdersAnalysisVM
{
    public Dictionary<string, decimal>? ordersStatusPercentages { get; set; }
    public List<OrdersOverLastTwelveMonthsVM>? ordersOverLastTwelveMonths { get; set; }
}
