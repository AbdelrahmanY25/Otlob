namespace Otlob.IServices;

public interface IOrderService
{
    Task<Result> PlaceOrder(PaymentMethod paymentMethod, string? specialNotes = null);
    Result<string> CreateStripeSession(string? specialNotes = null);
    Task<Result> FinishCreditPayment(string tempOrderId);
}