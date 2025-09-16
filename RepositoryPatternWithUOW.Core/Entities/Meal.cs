namespace Otlob.Core.Entities;

public class Meal : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsNewMeal { get; set; } = false;
    public bool IsTrendingMeal { get; set; } = false;
    public int NumberOfServings { get; set; } = 1;
    public string? Image { get; set; }

    [ValidateNever]
    public MenuCategory Category { get; set; } = null!;

    [ValidateNever]
    public Restaurant Restaurant { get; set; } = null!;

    [ValidateNever]
    public ICollection<MealPriceHistory> MealPriceHistories { get; set; } = null!;
}
