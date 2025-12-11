namespace Otlob.Core.Contracts.Documents;

public class TradeMarkResponse
{
    public string Id { get; init; } = string.Empty;
    public string RestaurantId { get; init; } = string.Empty;
    public string TrademarkName { get; init; } = string.Empty;
    public string TrademarkNumber { get; init; } = string.Empty;
    public DateOnly ExpiryDate { get; init; }
    public DocumentStatus DocumentStatus { get; init; }
    public FileResponse TradeMarkCertificate { get; init; } = default!;
}
