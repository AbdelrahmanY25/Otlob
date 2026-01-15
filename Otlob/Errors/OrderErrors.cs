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

    public static readonly Error CannotCancelOrder = 
        new("Order.CannotCancelOrder", "Order can only be cancelled while it is in Pending status.");

    public static readonly Error UnauthorizedCancellation = 
        new("Order.UnauthorizedCancellation", "You are not authorized to cancel this order.");

    public static readonly Error CancellationFailed =
        new("Order.CancellationFailed", "cancelation field try again");
}
