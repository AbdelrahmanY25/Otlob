namespace Otlob.Core.Contracts.MenuCategory;

public class MenuResponse
{
   public MenuCategoryResponse Categories { get; init; } = default!;
   public IEnumerable<MealResponse> Meals { get; init; } = default!;
}
