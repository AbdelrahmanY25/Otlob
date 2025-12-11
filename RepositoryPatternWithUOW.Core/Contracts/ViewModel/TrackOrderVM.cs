namespace Otlob.Core.Contracts.Authentication;

public class TrackOrderVM
{
    public string OrderId { get; set; } = null!;
    public OrderStatus OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public string RestaurantName { get; set; } = null!;
    public string? RestaurantImage { get; set; }
}
