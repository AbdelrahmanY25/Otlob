namespace Otlob.Core.Contracts.MealsAnalytics;

public class MealsAnalyticsResponse
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string MealId { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal Sales { get; set; }
    public string MealName { get; set; } = string.Empty;
    public string? MealImage { get; set; } = string.Empty;
}
