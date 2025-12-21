namespace Otlob.Core.Entities;

public class MealAddOn
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string MealId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Image { get; set; }

    public Meal Meal { get; set; } = default!;
}
