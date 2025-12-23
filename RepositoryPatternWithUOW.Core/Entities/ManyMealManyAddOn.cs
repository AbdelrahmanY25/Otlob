namespace Otlob.Core.Entities;

public class ManyMealManyAddOn
{
    public string MealId { get; set; } = string.Empty;
    public string AddOnId { get; set; } = string.Empty;

    public Meal Meal { get; set; } = default!;
    public MealAddOn AddOn { get; set; } = default!;
}