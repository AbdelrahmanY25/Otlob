namespace Otlob.Core.Contracts.Restaurant;

public class RestaurantDetailsResponse
{
    public string Key { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumberOfBranches { get; set; } = 1;
    public decimal DeliveryFee { get; set; }
    public decimal DeliveryDuration { get; set; }
    public AcctiveStatus AcctiveStatus { get; set; }
    public ProgressStatus ProgressStatus { get; set; }
    public BusinessType BusinessType { get; set; }
    public AdministratorRole AdministratorRole { get; set; }
    public string? Image { get; set; }
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public bool IsAvailable { get; set; }
    public List<Category> Categories { get; set; } = [];
}
