namespace Otlob.Core.Entities;

public class AuditEntity
{
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public string UpdatedById {  get; set; } = string.Empty;
    public DateTime UpdatedOn { get; set; }

    public ApplicationUser CreatedBy { get; set; } = null!;
    public ApplicationUser UpdatedBy { get; set;  } = null!;
}
