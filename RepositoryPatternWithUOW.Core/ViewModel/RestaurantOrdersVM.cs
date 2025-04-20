using Otlob.Core.Models;

namespace Otlob.Core.ViewModel
{
    public class RestaurantOrdersVM
    {
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalOrderPrice { get; set; }
    }
}
