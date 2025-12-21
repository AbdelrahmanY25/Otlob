namespace Otlob.Core.Contracts.MealVarients;

public class AddOnResponse
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Image { get; init; }
}
