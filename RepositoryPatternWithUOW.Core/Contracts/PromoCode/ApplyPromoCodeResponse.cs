namespace Otlob.Core.Contracts.PromoCode;

public class ApplyPromoCodeResponse
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal OriginalTotal { get; set; }
    public decimal NewTotal { get; set; }
    public string DiscountDisplay { get; set; } = string.Empty;
}
