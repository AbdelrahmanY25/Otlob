namespace Otlob.Core.Contracts.PromoCode;

public class PromoCodeRequest
{
    [Required(ErrorMessage = "Promo code is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 50 characters")]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code must contain only uppercase letters and numbers")]
    public string Code { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Discount type is required")]
    public DiscountType DiscountType { get; set; }

    [Required(ErrorMessage = "Discount value is required")]
    [Range(0.01, 100000, ErrorMessage = "Discount value must be greater than 0")]
    public decimal DiscountValue { get; set; }

    [Range(0, 100000, ErrorMessage = "Minimum order amount must be positive")]
    public decimal? MinOrderAmount { get; set; }

    [Range(0, 100000, ErrorMessage = "Maximum discount amount must be positive")]
    public decimal? MaxDiscountAmount { get; set; }

    [Required(ErrorMessage = "Valid from date is required")]
    public DateTime ValidFrom { get; set; }

    [Required(ErrorMessage = "Valid to date is required")]
    public DateTime ValidTo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Max total usage must be at least 1")]
    public int? MaxTotalUsage { get; set; }

    [Range(1, 100, ErrorMessage = "Max usage per user must be between 1 and 100")]
    public int MaxUsagePerUser { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public bool IsFirstOrderOnly { get; set; } = false;
}
