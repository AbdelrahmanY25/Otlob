namespace Otlob.Core.Entities;

public class PromoCode
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int MaxUsagePerUser { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public bool IsFirstOrderOnly { get; set; } = false;

    // Null means global code (SuperAdmin), otherwise restaurant-specific
    public int? RestaurantId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public Restaurant? Restaurant { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = default!;
    public ICollection<PromoCodeUsage> Usages { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
