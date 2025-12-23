namespace Otlob.Core.Contracts.MenuCategory;

public class MenuResponse
{
   public IEnumerable<CategoriesWithMealsResponse> CategoriesWithMeals { get; init; } = default!;
    public IEnumerable<AddOnResponse> AddOns { get; init; } = default!;
}
