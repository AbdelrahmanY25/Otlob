namespace Otlob.Core.Entities;

public sealed class CommercialRegistration : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string CertificateRegistrationId { get; set; } = string.Empty;

    public string RegistrationNumber { get; set; } = string.Empty;
    public DateOnly DateOfIssuance { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    
    public bool Expired => DateOnly.FromDateTime(DateTime.UtcNow) >= ExpiryDate;

    public Restaurant Restaurant { get; set; } = default!;
    public UploadedFile CertificateRegistration { get; set; } = default!;
}
