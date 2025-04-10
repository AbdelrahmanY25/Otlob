using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.IServices
{
    public interface IMealService
    {
        IQueryable<MealVm> ViewAllMealsVm(int RestaurantlId);
        IQueryable<MealVm> ViewMealsVmToRestaurantAdminSummary(int RestaurantlId);
        MealVm GetMealVM(int MealId);
        Meal GetMeal(int MealId);
        Meal GetMealNameAndImage(int mealId);
        Task<string> AddMeal(MealVm mealVM, int restaurantId, IFormFileCollection image);
        Task<string> EditMeal(MealVm mealVM, int mealId, IFormFileCollection image);
        bool DeleteMeal(int mealId);
        IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter);
    }
}
