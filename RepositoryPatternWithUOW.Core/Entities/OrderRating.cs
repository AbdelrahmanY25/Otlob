namespace Otlob.Core.Entities;

public class OrderRating
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    
    public bool GoodFood { get; set; }
    public bool FastDelivery { get; set; }
    public bool GreatPacking { get; set; }
    public bool FreshFood { get; set; }
    public bool GoodPortionSize { get; set; }
    public bool FriendlyDelivery { get; set; }
    public bool WorthThePrice { get; set; }
    
    public string? Comment { get; set; }
    public DateTime RatedAt { get; set; } = DateTime.Now;
    
    public Order Order { get; set; } = default!;
}