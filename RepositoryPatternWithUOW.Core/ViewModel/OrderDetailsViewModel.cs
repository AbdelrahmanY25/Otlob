using Otlob.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.ViewModel
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<MealsInOrder> Meals { get; set; }
        public decimal SubPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice => SubPrice + DeliveryFee;
    }
}
