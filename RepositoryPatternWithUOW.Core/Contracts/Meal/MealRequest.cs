namespace Otlob.Core.Contracts.Meal;

public class MealRequest
{
    public IQueryable<MenuCategoryResponse>? Categories { get; init; } = default!;
    public string SelectedCategoryKey { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public bool IsNewMeal { get; init; }
    public bool IsTrendingMeal { get; init; }
    public int NumberOfServings { get; init; }
}
