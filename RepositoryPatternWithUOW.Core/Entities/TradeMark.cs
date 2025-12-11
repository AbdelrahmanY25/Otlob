namespace Otlob.Core.Entities;

public sealed class TradeMark : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string TradeMarkCertificateId { get; set; } = string.Empty;

    public string TrademarkName { get; set; } = string.Empty;
    public string TrademarkNumber { get; set; } = string.Empty;
    public DateOnly ExpiryDate { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    
    public bool Expired => DateOnly.FromDateTime(DateTime.UtcNow) >= ExpiryDate;

    public Restaurant Restaurant { get; set; } = null!;
    public UploadedFile TradeMarkCertificate { get; set; } = null!;
}