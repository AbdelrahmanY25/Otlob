namespace Otlob.Core.Entities;

public class RestaurantCategory
{
    public int CategoryId { get; set; }
    public int RestaurantId { get; set; }

    public Category Category { get; set; } = default!;
    public Restaurant Restaurant { get; set; } = default!;
}
