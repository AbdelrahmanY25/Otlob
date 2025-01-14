using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Otlob.Core.Models;
using RepositoryPatternWithUOW.Core.Models;
using System.ComponentModel.DataAnnotations;
namespace Otlob.Core.ViewModel
{
    public class MealVm
    {
        public int Id { get; set; }

        [Required ,MinLength(3)]
        public string Name { get; set; }

        public int RestaurantId { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; }

        [Required, Range(0, 50000)]
        public decimal Price { get; set; }

        [Required, Range(0, 20)]
        public int NumberOfServings { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        public bool IsNewMeal { get; set; }
        public bool IsTrendingMeal { get; set; }

        [Required]
        public MealCategory Category { get; set; }

        public static MealVm MaptoMealVm(Meal meal)
        {
            return new MealVm
            {
                Name = meal.Name,
                RestaurantId = meal.RestaurantId,
                ImageUrl = meal.ImageUrl,
                Description = meal.Description,
                Price = meal.Price,
                NumberOfServings = meal.NumberOfServings,
                IsAvailable = meal.IsAvailable,
                IsNewMeal = meal.IsNewMeal,
                IsTrendingMeal = meal.IsTrendingMeal,
                Category = meal.Category
            };
        }

        public static Meal MapToMeal(MealVm mealVm, Meal oldMeal, ApplicationUser restaurant)
        {
            oldMeal.Name = mealVm.Name;
            oldMeal.Description = mealVm.Description;
            oldMeal.Price = mealVm.Price;
            oldMeal.Category = mealVm.Category;
            oldMeal.IsAvailable = mealVm.IsAvailable;
            oldMeal.IsNewMeal = mealVm.IsNewMeal;
            oldMeal.IsTrendingMeal = mealVm.IsTrendingMeal;
            oldMeal.ImageUrl = mealVm.ImageUrl;
            oldMeal.RestaurantId = restaurant.Resturant_Id;
            oldMeal.NumberOfServings = mealVm.NumberOfServings;

            return oldMeal;
        }
        public static Meal MapToMeal(MealVm mealVm, Meal oldMeal, int resId)
        {
            oldMeal.Name = mealVm.Name;
            oldMeal.Description = mealVm.Description;
            oldMeal.Price = mealVm.Price;
            oldMeal.Category = mealVm.Category;
            oldMeal.IsAvailable = mealVm.IsAvailable;
            oldMeal.IsNewMeal = mealVm.IsNewMeal;
            oldMeal.IsTrendingMeal = mealVm.IsTrendingMeal;
            oldMeal.ImageUrl = mealVm.ImageUrl;
            oldMeal.RestaurantId = resId;
            oldMeal.NumberOfServings = mealVm.NumberOfServings;

            return oldMeal;
        }

        public static Meal MapToMeal(MealVm mealVm, ApplicationUser restaurant)
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
                ImageUrl = mealVm.ImageUrl,
                RestaurantId = restaurant.Resturant_Id,
                NumberOfServings = mealVm.NumberOfServings
            };
        }
        public static Meal MapToMeal(MealVm mealVm, int resId)
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
                ImageUrl = mealVm.ImageUrl,
                RestaurantId = resId,
                NumberOfServings = mealVm.NumberOfServings
            };
        }

    }
}
