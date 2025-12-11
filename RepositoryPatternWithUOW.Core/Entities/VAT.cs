namespace Otlob.Core.Entities;

public sealed class VAT : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string VatCertificateId { get; set; } = string.Empty;   

    public string VatNumber { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    public Restaurant Restaurant { get; set; } = null!;
    public UploadedFile VatCertificate { get; set; } = null!;
}
