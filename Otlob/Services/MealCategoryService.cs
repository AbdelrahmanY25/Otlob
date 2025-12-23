namespace Otlob.Services;

public class MealCategoryService(IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService,
                                 IMealService mealService) : IMealCategoryService
{
    private readonly IMealService _mealService = mealService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Result<IQueryable<MenuCategoryResponse>>? GetAllByRestaurantId(int restaurantId)
    {     
        var response = _unitOfWorkRepository.MealCategories
            .GetAllWithSelect(
                expression: mc => mc.RestaurantId == restaurantId,
                tracked: false,
                includeProps: [mc => mc.Meals],
                selector: mc => new MenuCategoryResponse 
                { 
                    Key = _encryptionService.Encrypt(mc.Id), 
                    Name = mc.Name
                }
            )!;

        if (response is null)
            return Result.Failure<IQueryable<MenuCategoryResponse>>(RestaurantErrors.NotFound);
        
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
                selector: mc => new MenuCategoryResponse { Key = _encryptionService.Encrypt(mc.Id), Name = mc.Name }
            )!;

        return Result.Success(response);
    }

    public Result Add(int restaurantId, MenuCategoryRequest request)
    {
        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants.IsExist(r => r.Id == restaurantId);

        if (!isRestaurantIdExists)
            return Result.Failure<IQueryable<MenuCategoryRequest>>(RestaurantErrors.NotFound);

        var isCategoryNameexists = IsCategoryNameExistsOnAdd(restaurantId, request);

        if (isCategoryNameexists.IsFailure)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        MenuCategory mealCategory = new() { RestaurantId = restaurantId, Name = request.Name };        

        _unitOfWorkRepository.MealCategories.Add(mealCategory);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success(mealCategory);
    }

    public Result Update(string key, MenuCategoryRequest request)
    {
        // TODO: Handle Exception
        int id = _encryptionService.Decrypt(key);

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
        int categoryId = _encryptionService.Decrypt(key);

        var categoryIdExistsResult = IsCategoryIdExists(categoryId);
        
        if (categoryIdExistsResult.IsFailure)
            return Result.Failure(MealCategoriesErrors.NotFound);
        
        _mealService.DeleteManyMealsByCategoryId(categoryId);

        _unitOfWorkRepository.MealCategories.SoftDelete(mc => mc.Id == categoryId);
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result UnDelete(string key)
    {
        // TODO: Handle Exception
        int categoryId = _encryptionService.Decrypt(key);

        var categoryIdExistsResult = IsCategoryIdExists(categoryId);
        
        if (categoryIdExistsResult.IsFailure)
            return Result.Failure(MealCategoriesErrors.NotFound);

        _mealService.UnDeleteManyMealsByCategoryId(categoryId);
        
        _unitOfWorkRepository.MealCategories.UnSoftDelete(mc => mc.Id == categoryId);
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result IsCategoryIdExists(int id)
    {
        bool isExists = _unitOfWorkRepository.MealCategories.IsExist(mc => mc.Id == id, ignoreQueryFilter: true);

        if (!isExists)
            return Result.Failure(MealCategoriesErrors.NotFound);

        return Result.Success();
    }



    private Result IsCategoryNameExistsOnAdd(int restaurantId, MenuCategoryRequest request)
    {
        bool isCategoryNameExists = _unitOfWorkRepository.MealCategories
            .IsExist(mc => mc.Name == request.Name && mc.RestaurantId == restaurantId, ignoreQueryFilter: true);

        if (isCategoryNameExists)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        return Result.Success();
    }

    private Result IsCategoryNameExistsOnUpdate(int restaurantId, int categoryId, MenuCategoryRequest request)
    {
        bool isCategoryNameExists = _unitOfWorkRepository.MealCategories
            .IsExist(mc => mc.Name == request.Name && mc.RestaurantId == restaurantId && mc.Id != categoryId, ignoreQueryFilter: true);

        if (isCategoryNameExists)
            return Result.Failure(MealCategoriesErrors.DoublicatedCategoryName);

        return Result.Success();
    }
}
