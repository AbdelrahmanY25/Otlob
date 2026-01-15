namespace Otlob.Errors;

public static class PromoCodeErrors
{
    public static readonly Error NotFound = 
        new("PromoCode.NotFound", "Promo code not found.");

    public static readonly Error InvalidCode = 
        new("PromoCode.InvalidCode", "Invalid promo code.");

    public static readonly Error CodeAlreadyExists = 
        new("PromoCode.CodeAlreadyExists", "A promo code with this code already exists.");

    public static readonly Error Expired = 
        new("PromoCode.Expired", "This promo code has expired.");

    public static readonly Error NotYetValid = 
        new("PromoCode.NotYetValid", "This promo code is not yet valid.");

    public static readonly Error Inactive = 
        new("PromoCode.Inactive", "This promo code is currently inactive.");

    public static readonly Error MaxUsageReached = 
        new("PromoCode.MaxUsageReached", "This promo code has reached its maximum usage limit.");

    public static readonly Error UserMaxUsageReached = 
        new("PromoCode.UserMaxUsageReached", "You have already used this promo code the maximum number of times.");

    public static readonly Error MinOrderAmountNotMet = 
        new("PromoCode.MinOrderAmountNotMet", "Your order does not meet the minimum amount required for this promo code.");

    public static readonly Error RestaurantNotMatched = 
        new("PromoCode.RestaurantNotMatched", "This promo code is not valid for this restaurant.");

    public static readonly Error FirstOrderOnly = 
        new("PromoCode.FirstOrderOnly", "This promo code is only valid for first-time orders.");

    public static readonly Error RestaurantNotActive = 
        new("PromoCode.RestaurantNotActive", "You cannot create promo codes while your restaurant is not active. Please wait for your restaurant to be approved and activated.");

    public static readonly Error CannotModifyGlobalCode = 
        new("PromoCode.CannotModifyGlobalCode", "You are not authorized to modify global promo codes.");

    public static readonly Error CannotModifyOtherRestaurantCode = 
        new("PromoCode.CannotModifyOtherRestaurantCode", "You are not authorized to modify promo codes from other restaurants.");

    public static readonly Error InvalidDateRange = 
        new("PromoCode.InvalidDateRange", "Valid from date must be before valid to date.");

    public static readonly Error InvalidDiscountValue = 
        new("PromoCode.InvalidDiscountValue", "Percentage discount value must be between 1 and 100.");
}
