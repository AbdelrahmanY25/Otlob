namespace Otlob.Core.Entities;

public class Meal : AuditEntity
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public int RestaurantId { get; set; }
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OfferPrice { get; set; } = 0;
    public string? Image { get; set; }
    public int NumberOfServings { get; set; } = 1;
    public bool IsAvailable { get; set; } = true;
    public bool IsNewMeal { get; set; } = false;
    public bool IsTrendingMeal { get; set; } = false;
    public bool HasOptionGroup { get; set; } = false;
    public bool HasAddOn { get; set; } = false;

    public bool HasOffer => OfferPrice > 0 && OfferPrice < Price;

    public MenuCategory Category { get; set; } = default!;
    public Restaurant Restaurant { get; set; } = default!;
    public ICollection<MealPriceHistory> MealPriceHistories { get; set; } = [];
    public ICollection<MealOptionGroup> OptionGroups { get; set; } = [];
    public ICollection<ManyMealManyAddOn> MealAddOns { get; set; } = [];
    public ICollection<MealsAnalytic> MealsAnalytics { get; set; } = [];
}
