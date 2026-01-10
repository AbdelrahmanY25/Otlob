namespace Otlob.ApiErrors;

public static class OrderErrors
{
    public static readonly Error AddOrderField = 
        new("Order.AddOrderField", "There is something wrong, please try again.");
    
    public static readonly Error StripeSessionFailed = 
        new("Order.StripeSessionFailed", "Failed to create payment session. Please try again.");
    
    public static readonly Error SessionExpired = 
        new("Order.SessionExpired", "Payment session expired. Please try again.");

    public static readonly Error NotFound = 
        new("Order.NotFound", "Order not found.");

    public static readonly Error InvalidStatusTransition = 
        new("Order.InvalidStatusTransition", "Invalid status transition. Cannot change order status.");
}
