using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Otlob.Core.Models
{
     public class Order
     {
        public int Id { get; set; } 
        public int AddressId { get; set; }
        public int RestaurantId { get; set; }
        public decimal OrderPrice { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod Method {  get; set; }
        public string? Notes { get; set; }

        [ValidateNever]
        public Restaurant Restaurant { get; set; }

        [ValidateNever]
        public ICollection<MealsInOrder> MealsInOrder { get; set; }

        [ValidateNever]
        public Address Address { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Preparing,
        Shipped,
        Delivered
    }
    public enum PaymentMethod
    {
        Cash,
        Credit
    }
}
