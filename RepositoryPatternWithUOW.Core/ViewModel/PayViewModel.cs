using Otlob.Core.Models;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
