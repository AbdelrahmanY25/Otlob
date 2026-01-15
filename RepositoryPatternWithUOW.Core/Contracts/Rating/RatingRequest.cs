namespace Otlob.Core.Contracts.Rating;

public class RatingRequest
{
    public int OrderId { get; set; }
    public bool GoodFood { get; set; }
    public bool FastDelivery { get; set; }
    public bool GreatPacking { get; set; }
    public bool FreshFood { get; set; }
    public bool GoodPortionSize { get; set; }
    public bool FriendlyDelivery { get; set; }
    public bool WorthThePrice { get; set; }
    public string? Comment { get; set; }
}