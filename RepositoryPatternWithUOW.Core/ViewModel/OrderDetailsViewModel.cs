namespace Otlob.Core.ViewModel
{
    public class OrderDetailsViewModel
    {
        public PaymentMethod PaymentMethod { get; set; }
        public int RestaurantId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public IQueryable<OrderDetails> Meals { get; set; }
        public decimal SubPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice => SubPrice + DeliveryFee;
    }
}
