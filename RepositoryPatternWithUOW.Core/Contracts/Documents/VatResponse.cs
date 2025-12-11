namespace Otlob.Core.Contracts.Documents;

public class VatResponse
{
    public string Id { get; init; } = string.Empty;
    public string RestaurantId { get; init; } = string.Empty;
    public string VatNumber { get; init; } = string.Empty;
    public DocumentStatus DocumentStatus { get; init; }
    public FileResponse VatCertificate { get; init; } = default!;
}
