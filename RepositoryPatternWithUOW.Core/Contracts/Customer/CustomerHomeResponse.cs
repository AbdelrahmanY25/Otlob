namespace Otlob.Core.Contracts.Customer;

public class CustomerHomeResponse
{
    public IQueryable<PromoCoreResponse> PromoCores { get; init; } = default!;
    public IQueryable<CategoryResponse> Categories { get; init; } = default!;
    public IQueryable<AcctiveRestaurantResponse> Restaurants { get; init; } = default!;
    public List<ActiveAdvertisementResponse> Advertisements { get; init; } = [];
    public IEnumerable<TopMealsResponse> TopTenMealsSales { get; init; } = default!;
    public IEnumerable<TopSalesRestaurantsResponse> TopRestaurantsSales { get; init; } = default!;
    public IEnumerable<TopSalesRestaurantsResponse> TopRestaurantsRates { get; init; } = default!;
}
