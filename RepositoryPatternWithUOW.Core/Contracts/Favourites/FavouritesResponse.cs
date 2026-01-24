namespace Otlob.Core.Contracts.Favourites;

public class FavouritesResponse
{
    public string RestaurantKey { get; init; } = string.Empty;
    public string RestaurantName { get; init; } = string.Empty;
    public string? Image { get; init; }
    public string? CoverImage { get; init; }
    public decimal Rating { get; init; }
    public int RatesCount { get; init; }
    public IEnumerable<string> Categories { get; init; } = [];
    public decimal DeliveryFee { get; init; }
    public decimal DeliveryTime { get; init; }
}
