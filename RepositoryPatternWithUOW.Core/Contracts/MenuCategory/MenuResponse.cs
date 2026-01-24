namespace Otlob.Core.Contracts.MenuCategory;

public class MenuResponse
{
   public MenuCategoryResponse Categories { get; init; } = default!;
   public IEnumerable<MealForMenuResponse> Meals { get; init; } = default!;
}
