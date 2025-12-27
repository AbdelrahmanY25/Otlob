namespace Otlob.Core.Contracts.Customer;

public class CustomerHomeResponse
{
    public IQueryable<string> Categories { get; init; } = default!;
    public IQueryable<AcctiveRestaurantResponse> Restaurants { get; init; } = default!;
}
