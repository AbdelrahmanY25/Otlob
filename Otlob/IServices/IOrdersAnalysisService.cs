namespace Otlob.IServices
{
    public interface IOrdersAnalysisService
    {
        OrdersAnalysisVM OrdersAnalysis();
        Dictionary<string, decimal> GetCurrentOrdersStatusPercentage();
        IQueryable<OrdersOverLastTwelveMonthsVM> GetOrdersOverLastTwelveMonth();
    }
}