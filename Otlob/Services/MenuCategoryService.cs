namespace Otlob.Services;

public class MenuCategoryService(IUnitOfWorkRepository unitOfWorkRepository, IRestaurantService restaurantService,
                                 IMealService mealService, IDataProtectionProvider dataProtectionProvider) : IMenuCategoryService
{
    private readonly IMealService _mealService = mealService;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<IQueryable<MenuCategoryResponse>>? GetAllByRestaurantId(int restaurantId)
    {
        var isRestaurantIdExistsResult = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (isRestaurantIdExistsResult.IsFailure)
            return Result.Failure<IQueryable<MenuCategoryResponse>>(RestaurantErrors.NotFound);

        var response = _unitOfWorkRepository.MealCategories
            .GetAllWithSelect(
                expression: mc => mc.RestaurantId == restaurantId,
                tracked: false,
                selector: mc => new MenuCategoryResponse { Key = _dataProtector.Protect(mc.Id.ToString()), Name = mc.Name }
            )!;

        return Result.Success(response);
    }

    public Result<MenuCategoryResponse>? GetById(int id)
    {
        var categoryIdExistsResult = IsCategoryIdExists(id);

        if (categoryIdExistsResult.IsFailure)
            return Result.Failure<MenuCategoryResponse>(MealCategoriesErrors.NotFound);

        var response = _unitOfWorkRepository.MealCategories
            .GetOneWithSelect(
                expression: mc => mc.Id == id,
                tracked: false,
                selector: mc => new MenuCategoryResponse { Key = _dataProtector.Protect(mc.Id.ToString()), Name = mc.Name }
            )!;

        return Result.Success(response);
    }

    public Result Add(int restaurantId, MenuCategoryRequest request)
    {
        var isRestaurantIdExistsResult = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (isRestaurantIdExistsResult.IsFailure)
            return Result.Failure<IQueryable<MenuCategoryRequest>>(RestaurantErrors.NotFound);

        var isCategoryNameexists = IsCategoryNameExistsOnAdd(restaurantId, request);

        if (isCategoryNameexists.IsFailure)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        MenuCategory mealCategory = new() { RestaurantId = restaurantId, Name = request.Name };        

        _unitOfWorkRepository.MealCategories.Create(mealCategory);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success(mealCategory);
    }

    public Result Update(string key, MenuCategoryRequest request)
    {
        // TODO: Handle Exception
        int id = int.Parse(_dataProtector.Unprotect(key));

        var categoryIdExistsResult = IsCategoryIdExists(id);
        
        if (categoryIdExistsResult.IsFailure)
            return Result.Failure(MealCategoriesErrors.NotFound);

        var mealCategory = _unitOfWorkRepository.MealCategories.GetOne(expression: mc => mc.Id == id)!;

        var isCategoryNameexists = IsCategoryNameExistsOnUpdate(mealCategory.RestaurantId, mealCategory.Id, request);

        if (isCategoryNameexists.IsFailure)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        mealCategory.Name = request.Name;

        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result Delete(string key)
    {
        // TODO: Handle Exception
        int id = int.Parse(_dataProtector.Unprotect(key));

        var categoryIdExistsResult = IsCategoryIdExists(id);
        
        if (categoryIdExistsResult.IsFailure)
            return Result.Failure(MealCategoriesErrors.NotFound);
        
        var mealCategory = _unitOfWorkRepository.MealCategories.GetOne(expression: mc => mc.Id == id)!;
        
        _mealService.DeleteMealsRelatedToCategoryWith(id);
        _unitOfWorkRepository.MealCategories.SoftDelete(mc => mc.Id == id);
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result UnDelete(string key)
    {
        // TODO: Handle Exception
        int id = int.Parse(_dataProtector.Unprotect(key));

        var categoryIdExistsResult = IsCategoryIdExists(id);
        
        if (categoryIdExistsResult.IsFailure)
            return Result.Failure(MealCategoriesErrors.NotFound);
        
        _mealService.UnDeleteMealsRelatedToCategoryWith(id);
        _unitOfWorkRepository.MealCategories.UnSoftDelete(mc => mc.Id == id);
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result IsCategoryIdExists(int id)
    {
        bool isExists = _unitOfWorkRepository.MealCategories.IsExist(mc => mc.Id == id);

        if (!isExists)
            return Result.Failure(MealCategoriesErrors.NotFound);

        return Result.Success();
    }

    private Result IsCategoryNameExistsOnAdd(int restaurantId, MenuCategoryRequest request)
    {
        bool isCategoryNameExists = _unitOfWorkRepository.MealCategories
            .IsExist(mc => mc.Name == request.Name && mc.RestaurantId == restaurantId);

        if (isCategoryNameExists)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        return Result.Success();
    }

    private Result IsCategoryNameExistsOnUpdate(int restaurantId, int categoryId, MenuCategoryRequest request)
    {
        bool isCategoryNameExists = _unitOfWorkRepository.MealCategories
            .IsExist(mc => mc.Name == request.Name && mc.RestaurantId == restaurantId && mc.Id != categoryId);

        if (isCategoryNameExists)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        return Result.Success();
    }
}
