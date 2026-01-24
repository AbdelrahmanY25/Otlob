namespace Otlob.Core.Contracts.Advertisement;

public class PaymentHistoryResponse
{
    public Guid Id { get; set; }
    public Guid AdvertisementId { get; set; }
    public string AdvertisementTitle { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public AdvertisementPaymentStatus PaymentStatus { get; set; }
    public string PaymentStatusDisplay => PaymentStatus.ToString();
    public string? CardLast4 { get; set; }
    public string? CardBrand { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string AmountDisplay => $"{Amount:N0} {Currency}";
}
