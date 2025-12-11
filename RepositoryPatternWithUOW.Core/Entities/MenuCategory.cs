namespace Otlob.Core.Entities;

public class MenuCategory
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public Restaurant Restaurant { get; set; } = default!;
    public ICollection<Meal> Meals { get; set; } = [];
}
