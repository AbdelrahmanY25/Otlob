using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ResturantId { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        public ICollection<OrderedMeals> OrderedMeals { get; set; }
    }
}
