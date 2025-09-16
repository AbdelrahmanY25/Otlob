namespace Otlob.Core.Contracts.ViewModel;

public class OrderedMealsVM
{
    public int Id { get; set; }
    public int MealId { get; set; }
    public int CartId { get; set; }

    [Required,Range(1, 99)]
    public int Quantity { get; set; }

    [Required, Range(1, 50000), Precision(18, 4)]
    public decimal PricePerMeal { get; set; }
    public string MealName { get; set; } = null!;
    public string? MealDescription { get; set; }
    public string? Image { get; set; }        
}
