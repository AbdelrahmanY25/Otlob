namespace Otlob.Core.Contracts.Advertisement;

public class ActiveAdvertisementResponse
{
    public Guid Id { get; set; }
    public string RestaurantId { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantImage { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public int PlanDisplayOrder { get; set; }  // For sorting (Featured first)
}
