namespace Otlob.Core.Entities;

public class CartDetails
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public string MealId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PricePerMeal { get; set; }

    [ValidateNever]
    public Meal Meal { get; set; } = null!;

    [ValidateNever]
    public Cart Cart { get; set; } = null!;
}
