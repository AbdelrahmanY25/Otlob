namespace Otlob.IServices;

public interface IOrderDetailsService
{
    Result<OrderDetailsResponse> GetOrderDetails(int orderId);
}
