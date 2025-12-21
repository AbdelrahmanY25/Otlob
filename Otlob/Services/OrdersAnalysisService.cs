namespace Otlob.Services;

public class OrdersAnalysisService(IOrderService orderService) : IOrdersAnalysisService
{
    private readonly IOrderService orderService = orderService;

    public OrdersAnalysisVM OrdersAnalysis()
    {            
       OrdersAnalysisVM ordersAnalysisVM = new()
       {
            ordersStatusPercentages = GetCurrentOrdersStatusPercentage(),
            ordersOverLastTwelveMonths = [.. GetOrdersOverLastTwelveMonth()]
       };

        return ordersAnalysisVM;
    }

    public Dictionary<string, decimal> GetCurrentOrdersStatusPercentage()
    {
        var ordersStatusCount = orderService.GroupOrdersDayByStatus().ToList().Select(o => new KeyValuePair<OrderStatus, int>(o.Key, o.Count()));

        int allOrders = ordersStatusCount.Sum(o => o.Value);

        var ordersStatusCountDic = new Dictionary<string, decimal>();

        foreach (var orderStatus in ordersStatusCount)
        {
            ordersStatusCountDic.Add(orderStatus.Key.ToString(), Math.Round((decimal)orderStatus.Value / allOrders * 100, 2));
        }           

        return ordersStatusCountDic;
    }

    public IQueryable<OrdersOverLastTwelveMonthsVM> GetOrdersOverLastTwelveMonth()
    {
        DateTime lastTwelveMonths = DateTime.Now.AddMonths(-11).Date;

        var orders = orderService.GetOrdersByStatus(OrderStatus.Delivered)
            .Where(o => o.OrderDate >= lastTwelveMonths)
            .Select(
                o => new { o.OrderDate, o.TotalOrderPrice, }
            )
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(
                g => new OrdersOverLastTwelveMonthsVM
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalOrderPrice)
                }
            )
            .OrderBy(o => o.Year)
            .ThenBy(o => o.Month);

        return orders;
    }
}
