using Otlob.Core.Models;

namespace Otlob.Core.ViewModel
{
    public class TrackOrderVM
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string RestaurantName { get; set; }
        public byte[]? RestaurantImage { get; set; }
    }
}
