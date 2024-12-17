using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
     public class Order
    {
        public int Id { get; set; } 
        public string ApplicationUserId { get; set; } = null!; 
        public string CustomerAddres { get; set; } = null!; 
        public int RestaurantId { get; set; }
        public int CartInOrderId { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod Method {  get; set; }
        public decimal OrderPrice { get; set; }
        public string? Notes { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [ValidateNever]
        public Restaurant Restaurant { get; set; }

        [ValidateNever]
        public CartInOrder CartInOrder { get; set; }
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
