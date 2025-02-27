using Otlob.Core.Models;

namespace Otlob.Core.ViewModel
{
    public class PayViewModel
    {
        public IEnumerable<Cart> Carts { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public Dictionary<int, OrderedMeals> OrderedMeals { get; set; }
        public Dictionary<int, Restaurant> Restaurants { get; set; }
    }
}
