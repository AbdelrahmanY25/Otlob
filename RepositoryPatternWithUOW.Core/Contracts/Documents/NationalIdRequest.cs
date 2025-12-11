namespace Otlob.Core.Contracts.Documents;

public class NationalIdRequest
{
    public string NationalIdNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateOnly NationalIdExpiryDate { get; init; }
}
