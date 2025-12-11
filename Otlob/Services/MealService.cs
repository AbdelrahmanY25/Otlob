namespace Otlob.Services;

public class MealService(IMapper mapper, IFileService imageService,
                         IUnitOfWorkRepository unitOfWorkRepository,
                         IDataProtectionProvider dataProtectionProvider,
                         IMealPriceHistoryService mealPriceHistoryService, 
                         IRestaurantService restaurantService) : IMealService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _imageService = imageService;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<IQueryable<MealResponnse>>? GetAllByRestaurantId(int restaurantlId)
    {
        Result IsRestaurantIdExists = _restaurantService.IsRestaurantIdExists(restaurantlId);

        if (IsRestaurantIdExists.IsFailure)
            return Result.Failure<IQueryable<MealResponnse>>(RestaurantErrors.NotFound);

        var response = _unitOfWorkRepository.Meals
            .GetAllWithSelect
             (
                expression: m => m.RestaurantId == restaurantlId,
                tracked: false,
                selector: m => new MealResponnse
                {
                    Key = _dataProtector.Protect(m.Id.ToString()),
                    Name = m.Name,
                    Price = m.Price,
                    Image = m.Image,
                }
             )!;

        return Result.Success(response);
    }

    public Result<IQueryable<MealVm>> GetMealsDetails(int restaurantId)
    {
        Result validateRestaurantIdResult = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return Result.Failure<IQueryable<MealVm>>(RestaurantErrors.NotFound);
        }

        IQueryable<MealVm>? mealsVM = _unitOfWorkRepository.Meals
            .GetAllWithSelect
             (
                expression: m => m.RestaurantId == restaurantId && m.IsAvailable,
                tracked: false,
                selector: m => new MealVm
                {
                    MealVmId = m.Id,
                    Name = m.Name,
                    Image = m.Image,
                    Price = m.Price,
                    NumberOfServings = m.NumberOfServings,
                    IsNewMeal = m.IsNewMeal,
                    IsTrendingMeal = m.IsTrendingMeal,
                    Description = m.Description
                }
             );

        return Result.Success(mealsVM)!;
    }

    public Result<MealResponnse> GetForUpdate(string key)
    {
        int mealId = int.Parse(_dataProtector.Unprotect(key));

        Result isMealIdExists = IsMealIdExists(mealId);

        if (isMealIdExists.IsFailure)
            return Result.Failure<MealResponnse>(MealErrors.MealNotFound);

        var meal = _unitOfWorkRepository.Meals
            .GetOne(expression: m => m.Id == mealId, tracked: false, ignoreQueryFilter: true, includeProps: [m => m.Restaurant.MenueCategories]);

        var response = _mapper.Map<MealResponnse>(meal);
        response.CategoryKey = _dataProtector.Protect(response.CategoryKey);
        response.Key = _dataProtector.Protect(response.Key);
        response.Categories = meal!.Restaurant.MenueCategories
            .Select(mc => new MenuCategoryResponse
            {
                Key = _dataProtector.Protect(mc.Id.ToString()),
                Name = mc.Name
            })
            .AsQueryable();

        return Result.Success(response);
    }

    public Result Add(int restaurantId, MealRequest request, UploadImageRequest imageRequest)
    {
        // TODO: Handle Exception
        int categoryId = int.Parse(_dataProtector.Unprotect(request.SelectedCategoryKey));

        var result = ValidateMealOnAdd(restaurantId, categoryId, request);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        var isImageUploaded = _imageService.UploadImage(imageRequest.Image)!;

        if (isImageUploaded.IsFailure)
            return Result.Failure(isImageUploaded.Error);

        Meal meal = FillMealData(request, restaurantId, categoryId, isImageUploaded.Value);

        _unitOfWorkRepository.Meals.Create(meal);
        _unitOfWorkRepository.SaveChanges();

        _mealPriceHistoryService.AddMealPriceHistory(meal.Id, meal.Price);

        return Result.Success();
    }

    public Result Update(MealRequest request, string key, int restaurantId)
    {
        // TODO: Handle Exception
        int mealId = int.Parse(_dataProtector.Unprotect(key));
        int categoryId = int.Parse(_dataProtector.Unprotect(request.SelectedCategoryKey));

        var validMeal = ValidateMealOnUpdate(restaurantId, mealId, categoryId, request);

        if (validMeal.IsFailure)
            return Result.Failure(validMeal.Error);

        Meal oldMeal = _unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId)!;

        if (oldMeal.Price != request.Price)
            _mealPriceHistoryService.UpdateMealPriceHistory(mealId, request.Price);

        bool noNewData = ThereIsNewData(request, oldMeal, categoryId);

        if (noNewData)
            return Result.Failure(MealErrors.NoNewData);
        
        _mapper.Map(request, oldMeal);
        oldMeal.CategoryId = categoryId;

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangeMealImage(IFormFile image, string key)
    {
        // TODO: Handle Exception
        int mealId = int.Parse(_dataProtector.Unprotect(key));

        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
            return Result.Failure(MealErrors.MealNotFound);        

        var uploadImageResult = _imageService.UploadImage(image);

        if (uploadImageResult.IsFailure)
            return uploadImageResult;

        Meal meal = _unitOfWorkRepository
            .Meals
            .GetOneWithSelect(
                expression: m => m.Id == mealId,
                selector: m => new Meal
                {
                    Id = m.Id,
                    Image = m.Image
                }
            )!;

        _imageService.DeleteImageIfExist(meal.Image);

        meal.Image = uploadImageResult.Value;

        _unitOfWorkRepository.Meals.ModifyProperty(meal, r => r.Image!);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    

    public Result<IQueryable<MealVm>> GetDeletedMeals(int restaurantId)
    {
        Result validateRestaurantIdResult = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return Result.Failure<IQueryable<MealVm>>(RestaurantErrors.NotFound);
        }

        IQueryable<MealVm> mealVms = _unitOfWorkRepository.Meals.GetAllWithSelect
             (
                expression: m => m.RestaurantId == restaurantId && EFCore.Property<bool>(m, "IsDeleted"),
                tracked: false,
                ignoreQueryFilter: true,
                selector: m => new MealVm
                {
                    Key = _dataProtector.Protect(m.Id.ToString()),
                    Name = m.Name,
                    Image = m.Image,
                    Price = m.Price,                        
                }
             )!;

        return Result.Success(mealVms);
    }

    public Result DeleteMeal(int mealId)
    {
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
        {
            return Result.Failure(MealErrors.MealNotFound);
        }

        _unitOfWorkRepository.Meals.SoftDelete(expression: m => m.Id == mealId);
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result UnDeleteMeal(int mealId)
    {
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
        {
            return Result.Failure(MealErrors.MealNotFound);
        }

        _unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.Id == mealId);
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public void DeleteMealsRelatedToCategoryWith(int categoryId)
    {
        _unitOfWorkRepository.Meals.SoftDelete(expression: m => m.CategoryId == categoryId);
    }

    public void UnDeleteMealsRelatedToCategoryWith(int categoryId)
    {
        _unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.CategoryId == categoryId);
    }

    public IQueryable<MealVm> MealCategoryFilter(IQueryable<MealVm> meals, string filter)
    {
        if (!filter.IsNullOrEmpty() && filter.ToLower() != "all")
        {
            meals = filter.ToLower() switch
            {
                "new" => meals.Where(m => m.IsNewMeal),
                "trend" => meals.Where(m => m.IsTrendingMeal),                   
                "bakeries" => meals.Where(m => m.Description.Contains("Bakeries")),
                "drink" => meals.Where(m => m.Description.Contains("Drink")),
                _ => meals
            };
        }
        return meals;
    }


    private Result ValidateMealOnAdd(int restaurantId, int categoryId, MealRequest request)
    {
        Result isRestaurantIdExists = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (isRestaurantIdExists.IsFailure)
            return Result.Failure(RestaurantErrors.NotFound);

        bool isExists = _unitOfWorkRepository.MealCategories.IsExist(mc => mc.Id == categoryId);

        if (!isExists)
            return Result.Failure(MealCategoriesErrors.NotFound);

        bool isMealNameExists = _unitOfWorkRepository.Meals
            .IsExist(m => m.RestaurantId == restaurantId && m.Name == request.Name);

        if (isMealNameExists)
            return Result.Failure(MealErrors.DoublicatedMealName);

        return Result.Success();
    }

    private Result ValidateMealOnUpdate(int restaurantId, int mealId, int categoryId, MealRequest request)
    {
        Result isMealIdExists = IsMealIdExists(mealId);

        if (isMealIdExists.IsFailure)
            return Result.Failure(MealErrors.MealNotFound);

        bool isExists = _unitOfWorkRepository.MealCategories.IsExist(mc => mc.Id == categoryId);

        if (!isExists)
            return Result.Failure(MealCategoriesErrors.NotFound);

        bool isMealNameExists = _unitOfWorkRepository.Meals
            .IsExist(m => m.RestaurantId == restaurantId && m.Name == request.Name && m.Id != mealId);

        if (isMealNameExists)
            return Result.Failure(MealErrors.DoublicatedMealName);

        return Result.Success();
    }

    private Result IsMealIdExists(int mealId)
    {
        if (mealId <= 0)
        {
            return Result.Failure(RestaurantErrors.NotFound);
        }

        bool isRestaurantIdExists = _unitOfWorkRepository.Meals.IsExist(expression: r => r.Id == mealId);

        if (!isRestaurantIdExists)
        {
            return Result.Failure(RestaurantErrors.NotFound);
        }

        return Result.Success();
    }

    private static bool ThereIsNewData(MealRequest request, Meal oldMeal, int categoryId)
    {
        return  oldMeal.Name == request.Name && oldMeal.Description == request.Description &&
                oldMeal.Price == request.Price && oldMeal.NumberOfServings == request.NumberOfServings &&
                oldMeal.IsAvailable == request.IsAvailable && oldMeal.IsNewMeal == request.IsNewMeal &&
                oldMeal.IsTrendingMeal == request.IsTrendingMeal && oldMeal.CategoryId == categoryId;
    }

    private Meal FillMealData(MealRequest request, int restaurantId, int categoryId, string imageUrl)
    {
        Meal meal = _mapper.Map<Meal>(request);

        meal.RestaurantId = restaurantId;
        meal.CategoryId = categoryId;
        meal.Image = imageUrl;

        return meal;
    }
}
