namespace Otlob.Core.Entities;

public class OrderDetails
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string MealId { get; set; } = string.Empty;
    public string MealDetails { get; set; } = string.Empty;
    public int MealQuantity { get; set; }
    public decimal MealPrice { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal AddOnsPrice { get; set; }
    public decimal TotalPrice { get; }


    public Meal Meal { get; set; } = default!;    
    public Order Order { get; set; } = default!;
}
