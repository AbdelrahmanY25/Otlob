using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Otlob.Core.Models
{
    public class Meal : ImageProp
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsNewMeal { get; set; } = false;
        public bool IsTrendingMeal { get; set; } = false;
        public int NumberOfServings { get; set; }
        public MealCategory Category { get; set; }

        [ValidateNever]
        public Restaurant Restaurant { get; set; }

        [ValidateNever]
        public ICollection<MealPriceHistory> MealPriceHistories { get; set; }
    }

    public enum MealCategory
    {
        MainCourse,
        Appetizer,
        Grill,
        Drinks,
        Sandwiches,
        Soup,
        Icecream,
        Dessert,
        SpecialMeals,
        Offers,
        Others
    }
}
