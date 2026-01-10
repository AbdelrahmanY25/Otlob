namespace Otlob.Core.Entities;

public class CartDetails
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public string MealId { get; set; } = string.Empty;
    public string MealDetails { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal MealPrice { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal AddOnsPrice { get; set; }
    public decimal TotalPrice { get; }


    public Meal Meal { get; set; } = default!;
    public Cart Cart { get; set; } = default!;
}
