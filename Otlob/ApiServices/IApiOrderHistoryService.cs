namespace Otlob.ApiServices;

public interface IApiOrderHistoryService
{
    IEnumerable<OrdersHistoreResponse> GetOrderHistory();
    ApiResult<OrderDetailsResponse> GetOrderDetails(int orderId);
}
