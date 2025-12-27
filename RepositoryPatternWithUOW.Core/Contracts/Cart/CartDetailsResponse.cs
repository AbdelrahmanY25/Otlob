namespace Otlob.Core.Contracts.Cart;

public class CartDetailsResponse
{
    public int Id { get; init; }
    public string MealId { get; init; } = string.Empty;
    public string MealName { get; init; } = string.Empty;
    public string? MealImage { get; init; }
    public string MealDeteils { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal MealPrice { get; init; }
    public decimal ItemsPrice { get; init; }
    public decimal AddOnsPrice { get; init; }
    public decimal TotalPrice { get; init; }
}
