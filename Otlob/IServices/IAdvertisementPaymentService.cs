using Otlob.Abstractions;
using Otlob.Core.Contracts.Advertisement;

namespace Otlob.IServices;

public interface IAdvertisementPaymentService
{
    // Stripe Checkout Session Payment (like OrderService)
    Result<string> CreateStripeSession(Guid advertisementId, int restaurantId);
    Result FinishPayment(Guid advertisementId);
    Task<Result> HandleStripeWebhook(string json, string signature);

    // Refund (when rejected)
    Result RefundPayment(Guid advertisementId, string reason);

    // Restaurant Costs Dashboard
    Result<RestaurantCostsResponse> GetRestaurantCosts(int restaurantId);
    List<PaymentHistoryResponse> GetPaymentHistory(int restaurantId);

    // Super Admin Revenue Dashboard
    Result<RevenueReportResponse> GetRevenueReport(DateTime? startDate = null, DateTime? endDate = null);
}
