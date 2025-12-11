namespace Otlob.Core.Contracts.Documents;

public class CommercialRegistrationResponse
{
    public string Id { get; init; } = string.Empty;
    public string RestaurantId { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
    public DateOnly DateOfIssuance { get; init; }
    public DateOnly ExpiryDate { get; init; }
    public DocumentStatus DocumentStatus { get; init; }
    public FileResponse CertificateRegistration { get; init; } = default!;
}
