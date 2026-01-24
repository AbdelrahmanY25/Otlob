namespace Otlob.ApiErrors;

public static class OrderApiErrors
{
    public static readonly ApiError NotFound =         
        new("OrderErrors.NotFound", "Order not found.", StatusCodes.Status404NotFound);

    public static readonly ApiError RestaurantNotFound =
        new("OrderErrors.RestaurantNotFound", "Restaurant not found or is not available.", StatusCodes.Status404NotFound);

    public static readonly ApiError RestaurantClosed =
        new("OrderErrors.RestaurantClosed", "Restaurant is currently closed.", StatusCodes.Status400BadRequest);

    public static readonly ApiError AddressNotFound =
        new("OrderErrors.AddressNotFound", "Delivery address not found.", StatusCodes.Status404NotFound);

    public static readonly ApiError MealNotFound =
        new("OrderErrors.MealNotFound", "One or more meals not found or unavailable.", StatusCodes.Status404NotFound);

    public static readonly ApiError MealNotFromRestaurant =
        new("OrderErrors.MealNotFromRestaurant", "One or more meals do not belong to the selected restaurant.", StatusCodes.Status400BadRequest);

    public static readonly ApiError InvalidOptionItem =
        new("OrderErrors.InvalidOptionItem", "One or more selected option items are invalid.", StatusCodes.Status400BadRequest);

    public static readonly ApiError InvalidAddOn =
        new("OrderErrors.InvalidAddOn", "One or more selected add-ons are invalid.", StatusCodes.Status400BadRequest);

    public static readonly ApiError EmptyOrder =
        new("OrderErrors.EmptyOrder", "Order must contain at least one item.", StatusCodes.Status400BadRequest);

    public static readonly ApiError InvalidQuantity =
        new("OrderErrors.InvalidQuantity", "Item quantity must be at least 1.", StatusCodes.Status400BadRequest);

    public static readonly ApiError MinimumOrderNotMet =
        new("OrderErrors.MinimumOrderNotMet", "Order total does not meet the restaurant's minimum order requirement.", StatusCodes.Status400BadRequest);

    public static readonly ApiError OrderCreationFailed =
        new("OrderErrors.OrderCreationFailed", "Failed to create order. Please try again.", StatusCodes.Status500InternalServerError);

    public static readonly ApiError StripePaymentFailed =
        new("OrderErrors.StripePaymentFailed", "Failed to create payment session. Please try again.", StatusCodes.Status500InternalServerError);

    public static readonly ApiError InvalidPaymentSession =
        new("OrderErrors.InvalidPaymentSession", "Invalid or expired payment session.", StatusCodes.Status400BadRequest);

    public static readonly ApiError PaymentNotCompleted =
        new("OrderErrors.PaymentNotCompleted", "Payment has not been completed.", StatusCodes.Status400BadRequest);

    public static readonly ApiError InvalidPromoCode =
        new("OrderErrors.InvalidPromoCode", "The promo code is invalid or expired.", StatusCodes.Status400BadRequest);
}
