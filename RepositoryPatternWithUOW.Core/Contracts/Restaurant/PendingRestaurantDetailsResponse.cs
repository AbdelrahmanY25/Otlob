namespace Otlob.Core.Contracts.Restaurant;

public class PendingRestaurantDetailsResponse
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Image { get; set; }
}
