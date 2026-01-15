
namespace Otlob.IServices;

public interface IMealService
{
    Result<IQueryable<MealResponse>>? GetAllByRestaurantId(int restaurantlId);

    Result<IQueryable<MealResponse>>? GetAllByCategoryId(int categoryId);

    Result<MealResponse> GetForUpdate(string key);

    Task<Result> AddAsync(int restaurantId, MealRequest request, UploadImageRequest imageRequest);

    Task<Result> UpdateAsync(MealRequest request, string mealId, int restaurantId);
    
    Result UpdateMealImage(UploadImageRequest imageRequest, string key);

    Result<IQueryable<MealResponse>> GetDeletedMeals(int restaurantId);

    Result DeleteMeal(string id);

    Result UnDeleteMeal(string id);

    Result DeleteManyMealsByCategoryId(int categoryId);
    
    Result UnDeleteManyMealsByCategoryId(int categoryId);

    Result DeleteAllByRestaurantId(int restaurantId);

    Result UnDeleteAllByRestaurantId(int restaurantId);
}
