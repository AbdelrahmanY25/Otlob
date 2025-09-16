namespace Otlob.Core.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [ValidateNever]
    public ICollection<RestaurantCategory> RestaurantCategory { get; set; } = null!;
}
