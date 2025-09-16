namespace Otlob.Core.Entities;

public class MenuCategory
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    public ICollection<Meal> Meals { get; set; } = null!;
}
