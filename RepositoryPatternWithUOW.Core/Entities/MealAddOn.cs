namespace Otlob.Core.Entities;

public class MealAddOn
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public int RestaurantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Image { get; set; }

    public Restaurant Restaurant { get; set; } = default!;
    public ICollection<ManyMealManyAddOn> MealAddOns { get; set; } = [];
}
