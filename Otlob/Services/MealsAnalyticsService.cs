namespace Otlob.Services;

public class MealsAnalyticsService(IUnitOfWorkRepository unitOfWorkRepository) : IMealsAnalyticsService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;    

    public void UpdateSales(int restaurantId, string mealId, int quantity, decimal saleAmount)
    {
        Add(restaurantId, mealId);

        MealsAnalytic? mealsAnalytic = _unitOfWorkRepository.MealsAnalytics
            .GetOne(expression: ma => ma.RestaurantId == restaurantId && ma.MealId == mealId)!;
        
        mealsAnalytic.Sales += saleAmount;
        mealsAnalytic.SalesCount += quantity;
        
        _unitOfWorkRepository.MealsAnalytics.Update(mealsAnalytic);
        _unitOfWorkRepository.SaveChanges();
    }

    public IEnumerable<MealsAnalyticsResponse> GetAllByRestaurantId(int restaurantId)
    {
        bool isRestaurantHasMealsAnalytics = _unitOfWorkRepository.MealsAnalytics
            .IsExist(ma => ma.RestaurantId == restaurantId);

        if (!isRestaurantHasMealsAnalytics)
            return [];

            var mealsAnalytics = _unitOfWorkRepository.MealsAnalytics
            .GetAllWithSelect(
                expression: ma => ma.RestaurantId == restaurantId,
                tracked: false,
                selector: ma => new MealsAnalyticsResponse
                {
                    Id = ma.Id,
                    RestaurantId = ma.RestaurantId,
                    MealId = ma.MealId,
                    Sales = ma.Sales,
                    SalesCount = ma.SalesCount,
                    MealName = ma.Meal.Name,
                    MealImage = ma.Meal.Image
                }
            )!;
               
        return mealsAnalytics;
    }

    public IEnumerable<MealsAnalyticsResponse> GetTopTenSales(int restaurantId)
    {
        bool isRestaurantHasMealsAnalytics = _unitOfWorkRepository.MealsAnalytics
            .IsExist(ma => ma.RestaurantId == restaurantId);

        if (!isRestaurantHasMealsAnalytics)
            return [];

        var analytics = _unitOfWorkRepository.MealsAnalytics
            .GetAllWithSelect(
                expression: ma => ma.RestaurantId == restaurantId,
                tracked: false,
                selector: ma => new MealsAnalyticsResponse
                {
                    Id = ma.Id,
                    RestaurantId = ma.RestaurantId,
                    MealId = ma.MealId,
                    Sales = ma.Sales,
                    SalesCount = ma.SalesCount,
                    MealName = ma.Meal.Name,
                    MealImage = ma.Meal.Image
                }
            )!
            .OrderByDescending(ma => ma.Sales)
            .Skip(0)
            .Take(10);

        return analytics;
    }

    private void Add(int restaurantId, string mealId)
    {
        bool isExists = _unitOfWorkRepository.MealsAnalytics
            .IsExist(ma => ma.RestaurantId == restaurantId && ma.MealId == mealId);

        if (!isExists)
        {
            MealsAnalytic mealsAnalytic = new()
            {
                RestaurantId = restaurantId,
                MealId = mealId,
                Sales = 0,
                SalesCount = 0
            };

            _unitOfWorkRepository.MealsAnalytics.Add(mealsAnalytic);
            _unitOfWorkRepository.SaveChanges();
        }
    }
}
