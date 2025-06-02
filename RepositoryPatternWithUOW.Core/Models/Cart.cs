namespace Otlob.Core.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int RestaurantId { get; set; }

        [ValidateNever]
        public  ApplicationUser User { get; set; }

        [ValidateNever]
        public  ICollection<OrderedMeals> OrderedMeals { get; set; }

        [ValidateNever]
        public  Restaurant Restaurant { get; set; }
    }
}
