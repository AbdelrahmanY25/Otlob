namespace Otlob.Core.Entities;

public sealed class CommercialRegistration : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string RegistrationNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string CertificateRegistration { get; set; } = string.Empty;

    public Restaurant Restaurant { get; set; } = null!;
}
