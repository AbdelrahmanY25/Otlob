namespace Otlob.Core.Entities;

public class RestaurantCategory
{
    public int CategoryId { get; set; }
    public int RestaurantId { get; set; }

    [ValidateNever]
    public Category Category { get; set; } = null!;

    [ValidateNever]
    public Restaurant Restaurant { get; set; } = null!;
}
