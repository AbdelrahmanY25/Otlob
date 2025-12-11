namespace Otlob.Core.Contracts.Meal;

public class MealResponnse
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }   
    public string? Image { get; init; }
    public string CategoryKey { get; set; } = string.Empty;
    public bool IsAvailable { get; init; }
    public bool IsNewMeal { get; init; }
    public bool IsTrendingMeal { get; init; }
    public int NumberOfServings { get; init; }
    public IQueryable<MenuCategoryResponse>? Categories { get; set; } = default!;
}
