namespace Otlob.IServices;

public interface IOrderService
{
    Task<Result> PlaceOrder(PaymentMethod paymentMethod, string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0);
    Result<string> CreateStripeSession(string? specialNotes = null, int? promoCodeId = null, decimal discountAmount = 0);
    Task<Result> FinishCreditPayment(string tempOrderId);
}