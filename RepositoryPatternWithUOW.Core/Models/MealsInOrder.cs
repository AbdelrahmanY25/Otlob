using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class MealsInOrder
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int CartInOrderId { get; set; }
        public int MealId { get; set; }
        public string MealName { get; set; }
        public string? MealDescription { get; set; }
        public int Quantity { get; set; }

        [ValidateNever]
        public Meal Meal { get; set; }

        [ValidateNever]
        public CartInOrder CartInOrder { get; set; }
    }
}
