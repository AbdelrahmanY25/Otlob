using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class Delivery
    {

        public int Id {  get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Restaurant Restaurant { get; set; }
        public int RestaurantId { get; set; }
    }
}
