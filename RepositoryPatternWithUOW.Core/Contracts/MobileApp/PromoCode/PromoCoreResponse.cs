namespace Otlob.Core.Contracts.MobileApp.PromoCode;

public record PromoCoreResponse
(
    string Code,
    decimal DiscountAmount,
    decimal? MinimumOrderAmount,
    string Description,
    string Flag
);
