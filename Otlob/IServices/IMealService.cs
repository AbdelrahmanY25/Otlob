namespace Otlob.IServices;

public interface IMealService
{
    Result<IQueryable<MealVm>> GetMealsByRestaurantId(int restaurantlId);
    
    Result<IQueryable<MealVm>> GetMealsDetails(int restaurantlId);

    Result<MealVm> GetMealVM(int MealId);
        
    Meal GetMealNameAndImage(int mealId);
    
    Result AddMeal(MealVm mealVM, int restaurantId, IFormFile image);
    
    Result EditMeal(MealVm mealVM, int mealId);
    
    Result ChangeMealImage(IFormFile image, int mealId);

    Result<IQueryable<MealVm>> GetDeletedMeals(int restaurantId);

    Result DeleteMeal(int mealId);

    Result UnDeleteMeal(int mealId);
    
    IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter);
}
