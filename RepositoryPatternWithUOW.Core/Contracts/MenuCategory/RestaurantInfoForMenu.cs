namespace Otlob.Core.Contracts.MenuCategory;

public class RestaurantInfoForMenu
{
    public string Name { get; init; } = default!;
    public string? Image { get; init; } = default!;
    public string? CoverImage { get; init; } = default!;
    public decimal DeliveryFee { get; init; }
    public decimal DeliveryDuration { get; init; }
    public decimal MinOrderPrice { get; init; }
    public decimal Rating { get; init; }
    public int RatesCount { get; init; }
}
