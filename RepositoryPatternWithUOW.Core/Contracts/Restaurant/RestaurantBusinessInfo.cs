namespace Otlob.Core.Contracts.Restaurant;

public class RestaurantBusinessInfo
{
    public string RestaurntKey { get; set; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public decimal DeliveryDurationTime { get; init; }
    public int NumberOfBranches { get; init; }
    public TimeOnly OpeningTime { get; init; }
    public TimeOnly ClosingTime { get; init; }
    public BusinessType BusinessType { get; init; }
    public AdministratorRole AdministratorRole { get; init; }
    public List<Category>? RestaurantCategories { get; init; }
    public List<Category>? AllCategories { get; init; }
    public List<int>? SelectedCategoryIds { get; init; }
}
