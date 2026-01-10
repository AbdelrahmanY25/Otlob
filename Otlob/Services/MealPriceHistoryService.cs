namespace Otlob.Services;

public class MealPriceHistoryService(IUnitOfWorkRepository unitOfWorkRepository) : IMealPriceHistoryService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public void AddMealPriceHistory(string mealId, decimal price)
    {
        _unitOfWorkRepository.MealsPriceHistories.Add(new() { MealId = mealId, Price = price });
    }

    public IQueryable<MealPriceHistoryResponse>? GetMealPriceHistories(string mealId)
    {        
        var mealPH = _unitOfWorkRepository.MealsPriceHistories.GetAllWithSelect
        (
            selector: mph => new MealPriceHistoryResponse
            {
                Price = mph.Price,
                StartDate = mph.StartDate,
                EndDate = mph.EndDate
            },
            expression: mph => mph.MealId == mealId,
            tracked: false
        );

        if (mealPH is not null)
        {
            mealPH = mealPH.OrderByDescending(mp => mp.StartDate);
        }

        return mealPH;
    }

    public void UpdateMealPriceHistory(string mealId, decimal price)
    {
        MealPriceHistory? oldMealPriciesHistory = _unitOfWorkRepository.MealsPriceHistories
             .Get(expression: m => m.MealId == mealId)!
             .OrderByDescending(mh => mh.StartDate)
             .FirstOrDefault();

        if (oldMealPriciesHistory is not null)
            oldMealPriciesHistory.EndDate = DateTime.Now;

        _unitOfWorkRepository.MealsPriceHistories.Update(oldMealPriciesHistory!);

        AddMealPriceHistory(mealId, price);
    }
}
