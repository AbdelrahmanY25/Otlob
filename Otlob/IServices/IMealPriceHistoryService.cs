namespace Otlob.IServices;

public interface IMealPriceHistoryService
{
    void AddMealPriceHistory(string mealId, decimal price);
    IQueryable<MealPriceHistoryResponse>? GetMealPriceHistories(string mealId);
    void UpdateMealPriceHistory(string mealId, decimal price);
}