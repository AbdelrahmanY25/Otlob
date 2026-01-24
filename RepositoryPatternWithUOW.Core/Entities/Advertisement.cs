namespace Otlob.Core.Entities;

public sealed class Advertisement
{
    public Guid Id { get; set; }
    
    public int RestaurantId { get; set; }
    
    public int AdvertisementPlanId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string TitleAr { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string DescriptionAr { get; set; } = string.Empty;
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public AdvertisementStatus Status { get; set; } = AdvertisementStatus.PendingPayment;
    
    public string? RejectionReason { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
    
    public string? ReviewedByUserId { get; set; }

    // Navigation
    public Restaurant Restaurant { get; set; } = default!;
    
    public AdvertisementPlan AdvertisementPlan { get; set; } = default!;
    
    public ApplicationUser? ReviewedByUser { get; set; }
    
    public AdvertisementPayment? Payment { get; set; }
    
    public AdvertisementAnalytics? Analytics { get; set; }
}
