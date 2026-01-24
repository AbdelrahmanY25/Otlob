namespace Otlob.Core.Entities;

public sealed class AdvertisementPlan
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string NameAr { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string DescriptionAr { get; set; } = string.Empty;
    
    public decimal PricePerMonth { get; set; }
    
    public int DurationInDays { get; set; } = 30;
    
    public int DisplayOrder { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Advertisement> Advertisements { get; set; } = [];
}
