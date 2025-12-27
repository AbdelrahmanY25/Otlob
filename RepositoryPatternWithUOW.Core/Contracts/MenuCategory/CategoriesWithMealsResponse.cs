namespace Otlob.Core.Contracts.MenuCategory;

public class CategoriesWithMealsResponse
{
    MenuCategoryResponse Category { get; init; } = default!;
    public IEnumerable<MealResponse> Meals { get; init; } = default!;
}
