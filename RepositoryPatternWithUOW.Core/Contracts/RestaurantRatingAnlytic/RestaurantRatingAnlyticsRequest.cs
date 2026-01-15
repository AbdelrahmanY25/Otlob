namespace Otlob.Core.Contracts.RestaurantRatingAnlytic;

public class RestaurantRatingAnlyticsRequest
{
    public string RestaurantId { get; init; } = string.Empty;
    public decimal ScoreRate {  get; init; }
}
