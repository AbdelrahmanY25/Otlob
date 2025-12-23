namespace Otlob.Services;

public class ManyMealsManyAddOnsService(IUnitOfWorkRepository unitOfWorkRepository) : IManyMealsManyAddOnsService
{
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public async Task<Result> AddAsync(MealRequest request, int restaurantId, string mealId)
    {
        var addOnsIdsFromDb = _unitOfWorkRepository.MealAddOns
            .GetAllWithSelect
                (
                expression: ao => ao.RestaurantId == restaurantId,
                tracked: false,
                selector: ao => ao.Id
                )!
                .ToList();

        if (request.SelectedAddOns!.Except(addOnsIdsFromDb).Any())
            return Result.Failure(MealErrors.NoNewData);

        List<ManyMealManyAddOn> mealAddons = [];

        foreach (var addOnKey in request.SelectedAddOns!)
        {
            var mealAddOn = new ManyMealManyAddOn
            {
                MealId = mealId,
                AddOnId = addOnKey
            };

            mealAddons.Add(mealAddOn);
        }

        await _unitOfWorkRepository.ManyMealManyAddOns.AddRangeAsync(mealAddons);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(MealRequest request, int restaurantId, string mealId)
    {
        var existingAddOnIds = _unitOfWorkRepository.ManyMealManyAddOns
                .GetAllWithSelect
                 (
                    expression: ma => ma.MealId == mealId,
                    tracked: false,
                    selector: ma => ma.AddOnId
                 )!
                 .ToList();

        // Remove old AddOns
        if (existingAddOnIds.Count > 0)
        {
            var mealAddOnsToRemove = _unitOfWorkRepository.ManyMealManyAddOns
                .Get(expression: ma => ma.MealId == mealId)!;

            _unitOfWorkRepository.ManyMealManyAddOns.HardDeleteRange(mealAddOnsToRemove);
        }

        // Add new AddOns
        if (request.HasAddOns && request.SelectedAddOns is not null && request.SelectedAddOns!.Count > 0)
        {
            var addAddOnsResult = await AddAsync(request, restaurantId, mealId);

            if (addAddOnsResult.IsFailure)
                return Result.Failure(addAddOnsResult.Error);
        }

        return Result.Success();
    }
}
