namespace Otlob.Core.Contracts.Advertisement;

public class AdvertisementPlanResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public decimal PricePerMonth { get; set; }
    public int DurationInDays { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    
    public string PriceDisplay => $"{PricePerMonth:N0} EGP/month";
}
