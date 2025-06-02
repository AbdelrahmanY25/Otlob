namespace Otlob.Core.Models
{
    public class OrderedMeals
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int MealId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerMeal { get; set; }

        [ValidateNever]
        public  Meal Meal { get; set; }

        [ValidateNever]
        public  Cart Cart { get; set; }
    }
}
