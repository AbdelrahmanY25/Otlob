namespace Otlob.Core.ViewModel
{
    public class MealVm : ImageUrl
    {
        public int MealVmId { get; set; }
        public int RestaurantId { get; set; }

        [Required ,MinLength(3)]
        public string Name { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; }

        [Required, Range(1, 50000)]
        public decimal Price { get; set; }

        [Required, Range(0, 20)]
        public int NumberOfServings { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
        public bool IsNewMeal { get; set; }
        public bool IsTrendingMeal { get; set; }
        public byte[]? Image { get; set; }

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

        public static Meal MapToMeal(MealVm mealVm, Meal oldMeal)
        {
            oldMeal.Name = mealVm.Name;
            oldMeal.Description = mealVm.Description;
            oldMeal.Price = mealVm.Price;
            oldMeal.Category = mealVm.Category;
            oldMeal.IsAvailable = mealVm.IsAvailable;
            oldMeal.IsNewMeal = mealVm.IsNewMeal;
            oldMeal.IsTrendingMeal = mealVm.IsTrendingMeal;
            oldMeal.NumberOfServings = mealVm.NumberOfServings;

            return oldMeal;
        }
       
        public static Meal MapToMeal(MealVm mealVm, int restaurantId)
        {
            return new Meal
            {
                Name = mealVm.Name,
                Description = mealVm.Description,
                Price = mealVm.Price,
                Category = mealVm.Category,
                IsAvailable = mealVm.IsAvailable,
                IsNewMeal = mealVm.IsNewMeal,
                IsTrendingMeal = mealVm.IsTrendingMeal,
                Image = mealVm.Image,
                RestaurantId = restaurantId,
                NumberOfServings = mealVm.NumberOfServings
            };
        }       
    }
}
