namespace Otlob.Core.Contracts.MobileApp.TopRestaurants;

public record TopSalesRestaurantsResponse
(
    string RestaurantKey,
    string Name,
    string? Image,
    string? CoverImage,
    decimal Rates,
    decimal DeliveryFees,
    decimal DeliveryTime,
    decimal TotalOrdersSales
);
