namespace Otlob.Core.Contracts.Cart;

public class CartRequest
{
    public string MealId { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public List<string> OptionItemsIds { get; init; } = [];
    public List<string> AddOnsIds { get; init; } = [];
}
