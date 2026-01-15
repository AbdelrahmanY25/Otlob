namespace Otlob.Core.Contracts.Cart;

public class CartResponse
{
    public int Id { get; init; }
    public int RestaurantId { get; init; }
    public decimal MinimumOrderPrice { get; init; }
    public IEnumerable<CartDetailsResponse> CartDetails { get; init; } = [];
}
