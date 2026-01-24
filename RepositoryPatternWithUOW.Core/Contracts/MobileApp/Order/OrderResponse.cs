namespace Otlob.Core.Contracts.MobileApp.Order;

public record OrderResponse
(
    bool IsSuccess,
    string Message,
    int? OrderId = null,
    OrderStatus? Status = null,
    decimal? TotalAmount = null,
    
    // For Credit payment - Stripe Payment Intent client secret for mobile SDK
    string? PaymentIntentClientSecret = null,
    // For Credit payment - Stripe Ephemeral Key for mobile SDK
    string? EphemeralKey = null,
    // For Credit payment - Stripe Customer ID
    string? CustomerId = null,
    // For Credit payment - Temp order ID to confirm payment later
    string? TempOrderId = null
);
