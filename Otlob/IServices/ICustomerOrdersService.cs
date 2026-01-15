namespace Otlob.IServices;

public interface ICustomerOrdersService
{
    IEnumerable<OrderHistoryResponse> GetUserOrders();
    IEnumerable<OrderHistoryResponse> GetUserOrdersByUserId(string userId);
    Result CancelOrder(int orderId, CustomerCancelReason reason);
}
