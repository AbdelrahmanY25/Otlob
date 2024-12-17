using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class CartInOrder
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ResturantId { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        public ICollection<MealsInOrder> MealsInOrder { get; set; }
    }
}
