namespace Otlob.Core.Contracts.Restaurant;

public class AcctiveRestaurantResponse
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public decimal DeliveryDuration { get; init; }
    public string? Image { get; init; }
    public AcctiveStatus Status { get; init; }
    public TimeOnly OpeningTime { get; init; }
    public TimeOnly ClosingTime { get; init; }
    public BusinessType BusinessType { get; init; }
    public IEnumerable<string> Categories { get; init; } = default!;

    public bool IsOpen => (TimeOnly.FromDateTime(DateTime.Now) >= OpeningTime && TimeOnly.FromDateTime(DateTime.Now) < ClosingTime) ||
                           OpeningTime == ClosingTime;
    public bool IsClosed => !IsOpen;
}
