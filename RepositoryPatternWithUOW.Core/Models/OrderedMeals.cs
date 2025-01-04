using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Otlob.Core.Models
{
    public class OrderedMeals
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int MealId { get; set; }
        public string MealName { get; set; }    
        public string? MealDescription { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; }
        public int CartId { get; set; }        

        [ValidateNever]
        public Meal Meal { get; set; }

        [ValidateNever]
        public Cart Cart { get; set; }
    }
}
