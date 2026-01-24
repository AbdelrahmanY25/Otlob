namespace Otlob.Core.Entities;

public sealed class AdvertisementPayment
{
    public Guid Id { get; set; }
    
    public Guid AdvertisementId { get; set; }
    
    public int RestaurantId { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Currency { get; set; } = "EGP";
    
    public string? StripeSessionId { get; set; }
    
    public string? StripePaymentIntentId { get; set; }
    
    public string? StripeChargeId { get; set; }
    
    public AdvertisementPaymentStatus PaymentStatus { get; set; } = AdvertisementPaymentStatus.Pending;
    
    public string? PaymentMethod { get; set; }
    
    public string? CardLast4 { get; set; }
    
    public string? CardBrand { get; set; }
    
    public DateTime? PaidAt { get; set; }
    
    public DateTime? RefundedAt { get; set; }
    
    public string? RefundReason { get; set; }
    
    public string? StripeRefundId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Advertisement Advertisement { get; set; } = default!;
    
    public Restaurant Restaurant { get; set; } = default!;
}
