using Otlob.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.ViewModel
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public string ResturantId { get; set; }
        public decimal MostExpensiveOrder { get; set; }
    }
}
