namespace Otlob.Core.Entities;

public sealed class NationalId : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string NationalCardId { get; set; } = string.Empty;
    public string SignatureImageId { get; set; } = string.Empty;
    
    public string NationalIdNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly NationalIdExpiryDate { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    
    public bool Expired => DateOnly.FromDateTime(DateTime.UtcNow) >= NationalIdExpiryDate;

    public Restaurant Restaurant { get; set; } = null!;
    public UploadedFile NationalCard { get; set; } = null!;
    public UploadedFile SignatureImage { get; set; } = null!;
}
