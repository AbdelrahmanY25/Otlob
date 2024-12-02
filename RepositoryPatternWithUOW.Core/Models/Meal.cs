using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternWithUOW.Core.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsNewMeal { get; set; } = false;
        public bool IsTrendingMeal { get; set; } = false;
        public int NumberOfServings { get; set; } = 1;
        public enum MealCategory
        {
            Dessert,
            MainCourse,
            Grill
        }
        public MealCategory Category { get; set; }
        public Restaurant Restaurant { get; set; }
        public int RestaurantId { get; set; }

    }
}
