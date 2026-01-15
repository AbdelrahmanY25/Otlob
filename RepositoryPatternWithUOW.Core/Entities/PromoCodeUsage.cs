namespace Otlob.Core.Entities;

public class PromoCodeUsage
{
    public int Id { get; set; }
    public int PromoCodeId { get; set; }
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal DiscountApplied { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public PromoCode PromoCode { get; set; } = default!;
    public Order Order { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
}
