namespace Otlob.Core.Contracts.Meal;

public class MealResponse
{
    public string Key { get; init; } = string.Empty;
    public string RestaurantKey { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal OfferPrice { get; init; }
    public string? Image { get; init; }
    public string CategoryKey { get; set; } = string.Empty;
    public bool IsAvailable { get; init; }
    public bool IsNewMeal { get; init; }
    public bool IsTrendingMeal { get; init; }
    public bool HasOptionGroup { get; init; }
    public bool HasAddOns { get; init; }
    public int NumberOfServings { get; init; }
    
    public IEnumerable<MenuCategoryResponse>? Categories { get; set; } = default!;
    public IEnumerable<OptionGroupResponse>? OptionGroups { get; set; } = default!;
    public IEnumerable<AddOnResponse>? AddOns { get; set; } = default!;
    public IEnumerable<string>? SelectedAddOns { get; set; } = default!;
}
