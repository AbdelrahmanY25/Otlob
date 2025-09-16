namespace Otlob.Core.Entities;

public sealed class Contract : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ContractFile { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; } // percentage value (e.g., 0.15 = 15%)

    public Restaurant Restaurant { get; set; } = null!;
}