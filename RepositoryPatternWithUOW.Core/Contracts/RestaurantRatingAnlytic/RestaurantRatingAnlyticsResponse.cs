namespace Otlob.Core.Contracts.RestaurantRatingAnlytic;

public class RestaurantRatingAnlyticsResponse
{
    public string RestaurantId { get; init; } = string.Empty;
    public decimal ScoreRate { get; init; }
    public int RatingsCount { get; init; }
    public decimal AverageRate {  get; init; }
}
