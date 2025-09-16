namespace Otlob.Core.Entities;

public sealed class RestaurantBranch : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public Point Location { get; set; } = null!;
    public double DeliveryRadiusKm { get; set; }
    public string MangerName { get; set; } = string.Empty;
    public string MangerPhone { get; set; } = string.Empty;

    public Restaurant Restaurant { get; set; } = null!;
}
