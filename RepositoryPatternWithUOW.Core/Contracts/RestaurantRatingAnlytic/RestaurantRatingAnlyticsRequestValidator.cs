namespace Otlob.Core.Contracts.RestaurantRatingAnlytic;

public class RestaurantRatingAnlyticsRequestValidator : AbstractValidator<RestaurantRatingAnlyticsRequest>
{
    public RestaurantRatingAnlyticsRequestValidator()
    {
        RuleFor(r => r.ScoreRate)
            .GreaterThanOrEqualTo(0.5m)
            .LessThanOrEqualTo(5.00m);
    }
}
