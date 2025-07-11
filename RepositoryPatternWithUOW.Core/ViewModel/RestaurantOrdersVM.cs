namespace Otlob.Core.ViewModel
{
    public class RestaurantOrdersVM
    {
        public int OrderId { get; set; }
        public string Key { get; set; } = null!;
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = null!;
        public string? RestaurantImage { get; set; } 
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalMealsPrice { get; set; }
        public decimal TotalTaxPrice { get; set; }
        public decimal TotalOrderPrice { get; set; }
    }
}
