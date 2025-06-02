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
        IQueryable<MealVm> GetDeletedMeals(int restaurantId);
        bool DeleteMeal(int mealId);
        bool UnDeleteMeal(int mealId);
        IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter);
    }
}
