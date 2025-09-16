namespace Otlob.Core.Entities;

public sealed class VAT : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string VatNumber { get; set; } = string.Empty;
    public string VatCertificateImage { get; set; } = string.Empty;

    public Restaurant Restaurant { get; set; } = null!;
}
