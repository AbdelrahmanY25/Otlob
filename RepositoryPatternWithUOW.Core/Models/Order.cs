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
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
        public Meal Meal { get; set; }
        public int MealId { get; set; }        
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public DateTime OrderDate { get; set; }
        public enum OrderStatus
        {
            Pending,
            Preparing,
            Shipped,
            Delivered
        }
        public OrderStatus Status { get; set; } =OrderStatus.Pending;
        public  enum PaymentMethod
        {
            Cash,
            Credit
        }
        public PaymentMethod Method {  get; set; }



    }
}
