namespace Otlob.IServices;

public interface ICustomerOrdersService
{
    IEnumerable<OrderHistoryResponse> GetUserOrders();    
}
