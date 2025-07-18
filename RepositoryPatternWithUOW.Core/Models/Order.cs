﻿namespace Otlob.Core.Models
{
     public class Order
     {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string UserAddress { get; set; }
        public int RestaurantId { get; set; }
        public decimal TotalMealsPrice { get; set; }
        public decimal TotalTaxPrice { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public string? Notes { get; set; }
        public PaymentMethod Method {  get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [ValidateNever]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        public  Restaurant Restaurant { get; set; }

        [ValidateNever]
        public  ICollection<OrderDetails> MealsInOrder { get; set; }       
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
