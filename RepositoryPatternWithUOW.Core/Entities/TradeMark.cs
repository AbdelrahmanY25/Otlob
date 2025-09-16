namespace Otlob.Core.Entities;

public sealed class TradeMark : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string TrademarkName { get; set; } = string.Empty;
    public string TrademarkNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string TradeMarkCertificate { get; set; } = string.Empty;

    public Restaurant Restaurant { get; set; } = null!;
}