namespace Otlob.Core.Models
{
    public class Delivery
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RestaurantId { get; set; }

        [ValidateNever]
        public  Restaurant Restaurant { get; set; }
    }
}
