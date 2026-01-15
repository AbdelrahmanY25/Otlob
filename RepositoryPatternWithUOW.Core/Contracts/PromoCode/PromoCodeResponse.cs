namespace Otlob.Core.Contracts.PromoCode;

public class PromoCodeResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public string DiscountTypeDisplay => DiscountType.ToString();
    public decimal DiscountValue { get; set; }
    public string DiscountDisplay => DiscountType == DiscountType.Percentage 
        ? $"{DiscountValue}%" 
        : $"{DiscountValue:N2} L.E";
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int MaxUsagePerUser { get; set; }
    public bool IsActive { get; set; }
    public bool IsFirstOrderOnly { get; set; }
    public int? RestaurantId { get; set; }
    public string? RestaurantName { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TotalUsageCount { get; set; }
    public bool IsExpired => DateTime.Now > ValidTo;
    public bool IsNotStarted => DateTime.Now < ValidFrom;
    public string Status => !IsActive ? "Inactive" : IsExpired ? "Expired" : IsNotStarted ? "Scheduled" : "Active";
}
