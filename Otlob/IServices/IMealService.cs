namespace Otlob.IServices;

public interface IMealService
{
    Result<IQueryable<MealResponnse>>? GetAllByRestaurantId(int restaurantlId);
    
    Result<IQueryable<MealVm>> GetMealsDetails(int restaurantlId);

    Result<MealResponnse> GetForUpdate(string key);

    Result Add(int restaurantId, MealRequest request, UploadImageRequest imageRequest);
    
    Result Update(MealRequest request, string kry, int restaurantId);
    
    Result ChangeMealImage(IFormFile image, string key);

    Result<IQueryable<MealVm>> GetDeletedMeals(int restaurantId);

    Result DeleteMeal(int mealId);

    Result UnDeleteMeal(int mealId);

    void DeleteMealsRelatedToCategoryWith(int categoryId);

    void UnDeleteMealsRelatedToCategoryWith(int categoryId);

    IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter);
}
