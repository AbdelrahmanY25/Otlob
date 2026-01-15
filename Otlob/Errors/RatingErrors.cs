namespace Otlob.ApiErrors;

public static class RatingErrors
{
    public static readonly Error OrderNotFound = 
        new("Rating.OrderNotFound", "Order not found.");
    
    public static readonly Error OrderNotDelivered = 
        new("Rating.OrderNotDelivered", "Order must be delivered before rating.");
    
    public static readonly Error AlreadyRated = 
        new("Rating.AlreadyRated", "This order has already been rated.");
    
    public static readonly Error Unauthorized = 
        new("Rating.Unauthorized", "You are not authorized to rate this order.");
    
    public static readonly Error NoTagsSelected = 
        new("Rating.NoTagsSelected", "Please select at least one tag to describe your experience.");
    
    public static readonly Error SubmissionFailed = 
        new("Rating.SubmissionFailed", "Failed to submit rating. Please try again.");
        
    public static readonly Error RatingNotFound = 
        new("Rating.RatingNotFound", "Rating not found for this order.");
}
