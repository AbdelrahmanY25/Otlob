namespace Otlob.Core.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MealId { get; set; }
        public decimal MealPrice { get; set; }
        public int MealQuantity { get; set; }
        public decimal TotalPrice { get; set; }

        [ValidateNever]
        public Meal Meal { get; set; }

        [ValidateNever]
        public Order Order { get; set; }
    }
}
