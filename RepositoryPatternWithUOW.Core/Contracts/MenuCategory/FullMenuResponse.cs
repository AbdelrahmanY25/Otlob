namespace Otlob.Core.Contracts.MenuCategory;

public class FullMenuResponse
{
    public IQueryable<PromoCoreResponse> PromoCores { get; init; } = default!;
    public RestaurantInfoForMenu Restaurant { get; set; } = default!;
    public IEnumerable<MenuResponse> Menu { get; set; } = default!;
}
