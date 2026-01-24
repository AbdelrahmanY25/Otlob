namespace Otlob.Core.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }

    public ICollection<RestaurantCategory> RestaurantCategory { get; set; } = [];
}
