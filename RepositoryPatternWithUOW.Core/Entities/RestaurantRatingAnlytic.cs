namespace Otlob.Core.Entities;

public class RestaurantRatingAnlytic
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public decimal Score { get; set; }
    public int RatesCount { get; set; }

    public decimal AverageRate { get; }

    public Restaurant Restaurant { get; set; } = default!;
}
