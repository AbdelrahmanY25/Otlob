namespace Otlob.Core.ViewModel
{
    public class MealVm
    {
        public int MealVmId { get; set; }
        public string? Key { get; set; }
        public int RestaurantId { get; set; }

        [Required ,MinLength(3)]
        public string Name { get; set; } = null!;

        [Required, MinLength(3)]
        public string Description { get; set; } = null!;

        [Required, Range(1, 50000)]
        public decimal Price { get; set; }

        [Required, Range(0, 20)]
        public int NumberOfServings { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
        public bool IsNewMeal { get; set; }
        public bool IsTrendingMeal { get; set; }
        public string? Image { get; set; }

        [Required]
        public MealCategory Category { get; set; }

        public static MealVm MaptoMealVm(Meal meal)
        {
            return new MealVm
            {
                Name = meal.Name,
                Image = meal.Image,
                Description = meal.Description,
                Price = meal.Price,
                NumberOfServings = meal.NumberOfServings,
                IsAvailable = meal.IsAvailable,
                IsNewMeal = meal.IsNewMeal,
                IsTrendingMeal = meal.IsTrendingMeal,
                Category = meal.Category
            };
        }
    }
}
