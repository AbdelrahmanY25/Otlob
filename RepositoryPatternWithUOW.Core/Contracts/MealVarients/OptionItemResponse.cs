namespace Otlob.Core.Contracts.MealVarients;

public class OptionItemResponse
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Image { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPobular { get; init; }
}
