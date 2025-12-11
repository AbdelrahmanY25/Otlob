namespace Otlob.Core.Entities;

public class Meal : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsNewMeal { get; set; } = false;
    public bool IsTrendingMeal { get; set; } = false;
    public int NumberOfServings { get; set; } = 1;

    public MenuCategory Category { get; set; } = default!;
    public Restaurant Restaurant { get; set; } = default!;
    public ICollection<MealPriceHistory> MealPriceHistories { get; set; } = [];
}
