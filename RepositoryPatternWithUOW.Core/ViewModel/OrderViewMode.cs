using Otlob.Core.Models;

namespace Otlob.Core.ViewModel
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public string ResturantId { get; set; }
        public decimal MostExpensiveOrder { get; set; }
    }
}
