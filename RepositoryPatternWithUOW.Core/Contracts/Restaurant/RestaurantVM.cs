namespace Otlob.Core.Contracts.Restaurant;

public record RestaurantVM
{
    public string? Key { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DeliveryDuration { get; set; }
    public decimal DeliveryFee { get; set; }

    public AcctiveStatus AcctiveStatus { get; set; } = AcctiveStatus.UnAccepted;

    [ValidateNever]
    public List<Category> Categories { get; set; } = null!;

    [ValidateNever]
    public string? Image { get; set; }
}
