namespace Otlob.Core.Contracts.Documents;

public class NationalIdResponse
{
    public string Id { get; init; } = string.Empty;
    public string RestaurantId { get; init; } = string.Empty;
    public string NationalIdNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateOnly NationalIdExpiryDate { get; init; }
    public DocumentStatus Status { get; init; }
    public FileResponse NationalIdCard { get; init; } = default!;
    public FileResponse SignatureImage { get; init; } = default!;
}
