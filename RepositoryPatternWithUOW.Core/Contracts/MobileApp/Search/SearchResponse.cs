namespace Otlob.Core.Contracts.MobileApp.Search;

public record SearchResponse
(
    IEnumerable<RestaurantSearchResponse> Restaurant,
    IEnumerable<MealSearchResponse> Meals
);
