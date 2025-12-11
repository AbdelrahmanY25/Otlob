namespace Otlob.Core.Contracts.Documents;

public class CommercialRegistrationRequest
{
    public string RegistrationNumber { get; init; } = string.Empty;
    public DateOnly DateOfIssuance { get; init; }
    public DateOnly ExpiryDate { get; init; }
}
