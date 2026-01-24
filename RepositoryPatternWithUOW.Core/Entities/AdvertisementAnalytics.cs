namespace Otlob.Core.Entities;

public sealed class AdvertisementAnalytics
{
    public Guid Id { get; set; }
    
    public Guid AdvertisementId { get; set; }
    
    public int Views { get; set; }
    
    public int Clicks { get; set; }
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Computed properties
    public decimal ClickThroughRate => Views > 0 ? (decimal)Clicks / Views * 100 : 0;

    // Navigation
    public Advertisement Advertisement { get; set; } = default!;
}
