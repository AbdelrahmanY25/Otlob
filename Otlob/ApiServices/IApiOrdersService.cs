namespace Otlob.ApiServices;

public interface IApiOrdersService
{
    Task<OrderResponse> PlaceOrderAsync(OrderRequest request);
    Task<OrderResponse> ConfirmCreditPaymentAsync(string tempOrderId);
    void RemoveTempOrderAfterExpiry(string tempOrderId);
}
