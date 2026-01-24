namespace Otlob.Errors;

public static class AdvertisementErrors
{
    // Not Found Errors
    public static readonly Error NotFound = 
        new("Advertisement.NotFound", "Advertisement not found.");

    public static readonly Error PlanNotFound = 
        new("Advertisement.PlanNotFound", "Advertisement plan not found.");

    public static readonly Error PaymentNotFound = 
        new("Advertisement.PaymentNotFound", "Payment not found for this advertisement.");

    // Authorization Errors
    public static readonly Error NotOwner = 
        new("Advertisement.NotOwner", "You are not authorized to access this advertisement.");

    public static readonly Error RestaurantNotActive = 
        new("Advertisement.RestaurantNotActive", "Your restaurant must be active to create advertisements.");

    // Status Errors
    public static readonly Error CannotEditStatus = 
        new("Advertisement.CannotEditStatus", "Advertisement cannot be edited in its current status. Only pending payment or rejected ads can be edited.");

    public static readonly Error CannotDeleteStatus = 
        new("Advertisement.CannotDeleteStatus", "Advertisement cannot be deleted in its current status. Only pending payment or rejected ads can be deleted.");

    public static readonly Error AlreadyPaid = 
        new("Advertisement.AlreadyPaid", "This advertisement has already been paid for.");

    public static readonly Error NotPendingReview = 
        new("Advertisement.NotPendingReview", "This advertisement is not pending review.");

    public static readonly Error AlreadyApproved = 
        new("Advertisement.AlreadyApproved", "This advertisement has already been approved.");

    public static readonly Error AlreadyRejected = 
        new("Advertisement.AlreadyRejected", "This advertisement has already been rejected.");

    public static readonly Error NotApproved = 
        new("Advertisement.NotApproved", "Advertisement must be approved before it can become active.");

    public static readonly Error AlreadyActive = 
        new("Advertisement.AlreadyActive", "This advertisement is already active.");

    public static readonly Error AlreadyExpired = 
        new("Advertisement.AlreadyExpired", "This advertisement has already expired.");

    // Payment Errors
    public static readonly Error PaymentRequired = 
        new("Advertisement.PaymentRequired", "Payment is required before submitting for review.");

    public static readonly Error PaymentFailed = 
        new("Advertisement.PaymentFailed", "Payment processing failed. Please try again.");

    public static readonly Error PaymentNotSucceeded = 
        new("Advertisement.PaymentNotSucceeded", "Payment has not been completed successfully.");

    public static readonly Error NotPendingPayment = 
        new("Advertisement.NotPendingPayment", "This advertisement is not pending payment.");

    public static readonly Error RefundFailed = 
        new("Advertisement.RefundFailed", "Failed to process refund. Please contact support.");

    public static readonly Error RefundNotPossible = 
        new("Advertisement.RefundNotPossible", "Refund is not possible for this payment. No charge found.");

    public static readonly Error AlreadyRefunded = 
        new("Advertisement.AlreadyRefunded", "This payment has already been refunded.");

    // Restaurant Errors
    public static readonly Error RestaurantNotFound = 
        new("Advertisement.RestaurantNotFound", "Restaurant not found.");

    // Validation Errors
    public static readonly Error InvalidStartDate = 
        new("Advertisement.InvalidStartDate", "Start date must be in the future.");

    public static readonly Error InvalidDateRange = 
        new("Advertisement.InvalidDateRange", "Invalid date range specified.");

    public static readonly Error PlanNotActive = 
        new("Advertisement.PlanNotActive", "Selected plan is not currently available.");

    public static readonly Error ImageRequired = 
        new("Advertisement.ImageRequired", "Advertisement image is required.");

    // Limit Errors
    public static readonly Error MaxActiveAdsReached = 
        new("Advertisement.MaxActiveAdsReached", "You have reached the maximum number of active advertisements.");

    // Stripe Errors
    public static readonly Error StripeError = 
        new("Advertisement.StripeError", "An error occurred while processing your payment. Please try again.");

    public static readonly Error StripeSessionFailed = 
        new("Advertisement.StripeSessionFailed", "Failed to create payment session. Please try again.");

    public static Error StripeErrorWithMessage(string message) => 
        new("Advertisement.StripeError", $"Payment error: {message}");

    // Webhook Errors
    public static readonly Error InvalidWebhookSignature = 
        new("Advertisement.InvalidWebhookSignature", "Invalid webhook signature.");

    public static readonly Error WebhookProcessingFailed = 
        new("Advertisement.WebhookProcessingFailed", "Failed to process webhook event.");
}
