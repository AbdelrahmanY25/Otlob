namespace Otlob.IServices;

public interface IManyMealsManyAddOnsService
{
    Task<Result> AddAsync(MealRequest request, int restaurantId, string mealId);
    Task<Result> UpdateAsync(MealRequest request, int restaurantId, string mealId);
}
