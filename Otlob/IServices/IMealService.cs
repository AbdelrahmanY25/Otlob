namespace Otlob.IServices
{
    public interface IMealService
    {
        IQueryable<MealVm> ViewAllMealsVm(int RestaurantlId);
        IQueryable<MealVm> ViewMealsVmToRestaurantAdminSummary(int RestaurantlId);
        MealVm GetMealVM(int MealId);
        Meal GetMeal(int MealId);
        Meal GetMealNameAndImage(int mealId);
        string AddMeal(MealVm mealVM, int restaurantId, IFormFile image);
        string EditMeal(MealVm mealVM, int mealId);
        Meal GetMealImageById(int mealId);
        void UpdateMealImage(Meal meal, string imageUrl);
        IQueryable<MealVm> GetDeletedMeals(int restaurantId);
        bool DeleteMeal(int mealId);
        bool UnDeleteMeal(int mealId);
        IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter);
    }
}
