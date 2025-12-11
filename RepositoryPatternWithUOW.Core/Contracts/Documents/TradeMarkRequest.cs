namespace Otlob.Core.Contracts.Documents;

public class TradeMarkRequest
{
    public string TrademarkName { get; init; } = string.Empty;
    public string TrademarkNumber { get; init; } = string.Empty;
    public DateOnly ExpiryDate { get; init; }
}
