namespace Otlob.Errors;

public static class CartErrors
{
    public static readonly Error NotFound =
        new("Cart.NotFound", "The specified cart was not found.");
    
    public static readonly Error DifferentRestaurant =
        new("Cart.DifferentRestaurant", "You can only order from one restaurant at a time.");
    
    public static readonly Error InvalidQuantity = 
        new("Cart.InvalidQuantity", "The quantity specified is invalid.");

    public static readonly Error AddToCartFailed = 
        new("Cart.AddToCartFailed", "Failed to add the item to the cart.");

    public static readonly Error UserNotFound  = 
        new("Cart.UserNotFound", "Login first to can add your favourites meals to cart.");

    public static readonly Error RemoveItemFailed = 
        new("Cart.RemoveItemFailed", "Failed to remove the item from the cart.");

    public static readonly Error DeleteFailed = 
        new("Cart.DeleteFailed", "Failed to delete the cart.");
}
