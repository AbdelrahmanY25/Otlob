namespace Otlob.Core.Entities;

public class OrderDetails
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string MealId { get; set; } = string.Empty;
    public decimal MealPrice { get; set; }
    public int MealQuantity { get; set; }
    public decimal TotalPrice { get; set; }

    
    public Meal Meal { get; set; } = null!;

    
    public Order Order { get; set; } = null!;
}
