using Otlob.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.ViewModel
{
    public class OrderVM
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = null!;

        [Required]
        public string CustomerAddres { get; set; } = null!;

        [Required]
        public int ResturantId { get; set; }

        [Required]
        public int CartId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public decimal OrderPrice { get; set; }
        public string? Notes { get; set; }
    }
}
