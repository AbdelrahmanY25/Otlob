
namespace Otlob.Core.Models
{
    public class MealPriceHistory
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } = null;
        public Meal Meal { get; set; }
    }
}
