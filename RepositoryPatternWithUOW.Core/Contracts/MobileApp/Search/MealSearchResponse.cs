namespace Otlob.Core.Contracts.MobileApp.Search;

public record MealSearchResponse
(
    string Key,
    string RestaurantKey,
    string Name,
    string Description,
    string? Image,
    decimal Price,
    decimal OfferPrice,
    bool IsNewMeal,
    bool IsTrendingMeal
);