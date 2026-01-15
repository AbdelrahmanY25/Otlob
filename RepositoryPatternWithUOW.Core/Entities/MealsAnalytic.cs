namespace Otlob.Core.Entities;

public class MealsAnalytic
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string MealId { get; set; } = string.Empty;     
    public int SalesCount { get; set; }
    public decimal Sales { get; set; }
    
    public Meal Meal { get; set; } = default!;
    public Restaurant Restaurant { get; set; } = default!;
}
