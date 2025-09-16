namespace Otlob.Core.Entities;

public sealed class NationalId : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public string NationalIdNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime NationalIdExpiryDate { get; set; }
    public string FrontNationalIdImage { get; set; } = string.Empty;
    public string BackNationalIdImage { get; set; } = string.Empty;
    public string SignatureImage { get; set; } = string.Empty;

    public Restaurant Restaurant { get; set; } = null!;
}
