namespace Otlob.Core.Entities;

public class AuditEntity
{
    public string? CreatedById { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public string? UpdatedById {  get; set; }
    public DateTime UpdatedOn { get; set; }

    public ApplicationUser CreatedBy { get; set; } = null!;
    public ApplicationUser UpdatedBy { get; set;  } = null!;
}
