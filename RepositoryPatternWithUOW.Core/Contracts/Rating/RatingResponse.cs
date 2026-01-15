namespace Otlob.Core.Contracts.Rating;

public class RatingResponse
{
    public int OrderId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantImage { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    
    public bool GoodFood { get; set; }
    public bool FastDelivery { get; set; }
    public bool GreatPacking { get; set; }
    public bool FreshFood { get; set; }
    public bool GoodPortionSize { get; set; }
    public bool FriendlyDelivery { get; set; }
    public bool WorthThePrice { get; set; }
    public string? Comment { get; set; }
    public DateTime? RatedAt { get; set; }
    public bool IsRated { get; set; }
    
    public List<string> SelectedTags
    {
        get
        {
            var tags = new List<string>();
            if (GoodFood) tags.Add("Good Food");
            if (FastDelivery) tags.Add("Fast Delivery");
            if (GreatPacking) tags.Add("Great Packing");
            if (FreshFood) tags.Add("Fresh Food");
            if (GoodPortionSize) tags.Add("Good Portion Size");
            if (FriendlyDelivery) tags.Add("Friendly Delivery");
            if (WorthThePrice) tags.Add("Worth The Price");
            return tags;
        }
    }
}
