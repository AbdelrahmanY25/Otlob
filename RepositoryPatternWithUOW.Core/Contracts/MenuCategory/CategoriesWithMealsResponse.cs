namespace Otlob.Core.Contracts.MenuCategory;

public class CategoriesWithMealsResponse
{
    public string CategoryKey { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public IEnumerable<MealResponse> Meals { get; init; } = default!;
}
