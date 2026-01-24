namespace Otlob.Core.Contracts.MobileApp.PromoCode;

public record ApplyPromoCodeRequest
(
    string Code,
    string RestaurantKey,
    decimal OrderAmount
);
