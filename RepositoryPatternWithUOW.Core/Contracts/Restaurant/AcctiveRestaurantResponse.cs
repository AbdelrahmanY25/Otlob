namespace Otlob.Core.Contracts.Restaurant;

public class AcctiveRestaurantResponse
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Image { get; set; }
    public AcctiveStatus Status { get; set; }
}
