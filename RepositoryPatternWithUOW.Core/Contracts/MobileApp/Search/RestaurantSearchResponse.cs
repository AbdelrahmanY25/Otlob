namespace Otlob.Core.Contracts.MobileApp.Search;

public record RestaurantSearchResponse
(
    string Key,
    string Name,
    string? Image,
    string? CoverImage,
    decimal Rating,
    int RatesCount,
    decimal DeliveryFee,
    decimal DeliveryDuration,
    IEnumerable<string> Categories
);
