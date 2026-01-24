namespace Otlob.Core.Contracts.MenuCategory;

public class MealForMenuResponse
{
    public string Key { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string? Image { get; init; } = default!;
    public decimal Price { get; init; }
    public decimal OfferPrice { get; init; }
    public bool IsAvailable { get; init; }
}
