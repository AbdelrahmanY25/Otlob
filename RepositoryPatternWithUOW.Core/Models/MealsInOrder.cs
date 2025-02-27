using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Otlob.Core.Models
{
    public class MealsInOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MealId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        [ValidateNever]
        public Meal Meal { get; set; }

        [ValidateNever]
        public Order Order { get; set; }
    }
}
