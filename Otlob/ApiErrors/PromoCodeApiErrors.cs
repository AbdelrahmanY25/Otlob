namespace Otlob.ApiErrors;

public static class PromoCodeApiErrors
{
    public static readonly ApiError NotFound = 
        new("PromoCode.NotFound", "Promo code not found.", StatusCodes.Status404NotFound);

    public static readonly ApiError InvalidCode = 
        new("PromoCode.InvalidCode", "Invalid promo code.", StatusCodes.Status400BadRequest);

    public static readonly ApiError CodeAlreadyExists = 
        new("PromoCode.CodeAlreadyExists", "A promo code with this code already exists.", StatusCodes.Status409Conflict);

    public static readonly ApiError Expired = 
        new("PromoCode.Expired", "This promo code has expired.", StatusCodes.Status400BadRequest);

    public static readonly ApiError NotYetValid = 
        new("PromoCode.NotYetValid", "This promo code is not yet valid.", StatusCodes.Status400BadRequest);

    public static readonly ApiError Inactive = 
        new("PromoCode.Inactive", "This promo code is currently inactive.", StatusCodes.Status400BadRequest);

    public static readonly ApiError MaxUsageReached = 
        new("PromoCode.MaxUsageReached", "This promo code has reached its maximum usage limit.", StatusCodes.Status400BadRequest);

    public static readonly ApiError UserMaxUsageReached = 
        new("PromoCode.UserMaxUsageReached", "You have already used this promo code the maximum number of times.", StatusCodes.Status400BadRequest);

    public static readonly ApiError MinOrderAmountNotMet = 
        new("PromoCode.MinOrderAmountNotMet", "Your order does not meet the minimum amount required for this promo code.", StatusCodes.Status400BadRequest);

    public static readonly ApiError RestaurantNotMatched = 
        new("PromoCode.RestaurantNotMatched", "This promo code is not valid for this restaurant.", StatusCodes.Status400BadRequest);

    public static readonly ApiError FirstOrderOnly = 
        new("PromoCode.FirstOrderOnly", "This promo code is only valid for first-time orders.", StatusCodes.Status400BadRequest);

    public static readonly ApiError RestaurantNotActive = 
        new("PromoCode.RestaurantNotActive", "You cannot create promo codes while your restaurant is not active. Please wait for your restaurant to be approved and activated.", StatusCodes.Status400BadRequest);

    public static readonly ApiError CannotModifyGlobalCode = 
        new("PromoCode.CannotModifyGlobalCode", "You are not authorized to modify global promo codes.", StatusCodes.Status403Forbidden);

    public static readonly ApiError CannotModifyOtherRestaurantCode = 
        new("PromoCode.CannotModifyOtherRestaurantCode", "You are not authorized to modify promo codes from other restaurants.", StatusCodes.Status403Forbidden);

    public static readonly ApiError InvalidDateRange = 
        new("PromoCode.InvalidDateRange", "Valid from date must be before valid to date.", StatusCodes.Status400BadRequest);

        public static readonly ApiError InvalidDiscountValue = 
            new("PromoCode.InvalidDiscountValue", "Percentage discount value must be between 1 and 100.", StatusCodes.Status400BadRequest);
    }