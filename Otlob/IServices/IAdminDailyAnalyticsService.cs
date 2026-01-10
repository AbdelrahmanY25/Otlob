namespace Otlob.IServices;

public interface IAdminDailyAnalyticsService
{
    void Add();
    void InitialUpdate();
    void UpdatePreparingOrders();
    void UpdateShippedOrders();
    void UpdateDeliveredOrders(decimal orderTotalPrice);
    void UpdateCancelledOrders();
    AdminDailyAnalyticsResponse? GetToDay();
    AdminDailyAnalyticsResponse? GetByDate(DateOnly date);
}
