namespace Otlob.IServices
{
    public interface IMealPriceHistoryService
    {
        bool AddMealPriceHistory(int mealId, decimal price);
        IQueryable<MealPriceHistoryVM>? GetMealPriceHistories(int mealId);
        bool UpdateMealPriceHistory(int mealId, decimal price);
    }
}