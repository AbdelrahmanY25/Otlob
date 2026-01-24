namespace Otlob.Core.Contracts.MobileApp.TopMeals;

public record TopMealsResponse
(
    string Id,
    string Name,
    string? Image,
    decimal Price,
    int SalesCount
);
