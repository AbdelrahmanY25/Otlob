namespace Otlob.Core.Contracts.Advertisement;

public class AdvertisementResponse
{
    public Guid Id { get; set; }
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public int AdvertisementPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public AdvertisementStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();
    public string? RejectionReason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByUserName { get; set; }
    
    // Analytics
    public int Views { get; set; }
    public int Clicks { get; set; }
    public decimal ClickThroughRate => Views > 0 ? (decimal)Clicks / Views * 100 : 0;
    
    // Payment info
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }

    // Computed
    public bool IsActive => Status == AdvertisementStatus.Active;
    public bool CanEdit => Status == AdvertisementStatus.PendingPayment || Status == AdvertisementStatus.Rejected;
    public bool CanDelete => Status == AdvertisementStatus.PendingPayment || Status == AdvertisementStatus.Rejected;
    public int DaysRemaining => Status == AdvertisementStatus.Active ? (EndDate - DateTime.UtcNow).Days : 0;
}
