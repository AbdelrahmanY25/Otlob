namespace Otlob.Services;

public class MealService(IMapper mapper, IImageService imageService,
                   IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider,
                   IMealPriceHistoryService mealPriceHistoryService) : IMealService
{
    private readonly IMapper _mapper = mapper;
    private readonly IImageService _imageService = imageService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<IQueryable<MealVm>> GetMealsByRestaurantId(int restaurantlId)
    {
        Result validateRestaurantIdResult = IsRestaurantIdExists(restaurantlId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return Result.Failure<IQueryable<MealVm>>(RestaurantErrors.InvalidRestaurantId);
        }

        IQueryable<MealVm> mealsVM = _unitOfWorkRepository.Meals
            .GetAllWithSelect
             (
                expression: m => m.RestaurantId == restaurantlId,
                tracked: false,
                selector: m => new MealVm
                {
                    Key = _dataProtector.Protect(m.Id.ToString()),
                    Name = m.Name,
                    Image = m.Image,
                    Price = m.Price,
                }
             )!;

        return Result.Success(mealsVM);
    }

    public Result<IQueryable<MealVm>> GetMealsDetails(int restaurantId)
    {
        Result validateRestaurantIdResult = IsRestaurantIdExists(restaurantId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return Result.Failure<IQueryable<MealVm>>(RestaurantErrors.InvalidRestaurantId);
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
                    Category = m.Category,
                    NumberOfServings = m.NumberOfServings,
                    IsNewMeal = m.IsNewMeal,
                    IsTrendingMeal = m.IsTrendingMeal,
                    Description = m.Description
                }
             );

        return Result.Success(mealsVM)!;
    }

    public Result<MealVm> GetMealVM(int mealId)
    {
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
        {
            return Result.Failure<MealVm>(MealErrors.MealNotFound);
        }

        var meal = _unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId, ignoreQueryFilter: true);

        return MaptoMealVm(meal!);
    }

    public Meal GetMealNameAndImage(int mealId) => 
        _unitOfWorkRepository.Meals
            .GetOneWithSelect(selector: m => new Meal { Id = m.Id, Name = m.Name, Image = m.Image}, expression: m => m.Id == mealId)!;

    public Result AddMeal(MealVm mealVM, int restaurantId, IFormFile image)
    {
        Result validateRestaurantIdResult = IsRestaurantIdExists(restaurantId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return validateRestaurantIdResult;
        }

        bool isMealNameExists = _unitOfWorkRepository.Meals.IsExist(m => m.RestaurantId == restaurantId && m.Name == mealVM.Name);

        if (isMealNameExists)
        {
            return Result.Failure(MealErrors.DoublicatedMealName);
        }

        var isImageUploaded = _imageService.UploadImage(image)!;

        if (isImageUploaded.IsFailure)
        {
            return Result.Failure(isImageUploaded.Error);
        }

        Meal meal = FillMealData(mealVM, restaurantId, isImageUploaded.Value);

        _unitOfWorkRepository.Meals.Create(meal);
        _unitOfWorkRepository.SaveChanges();

        _mealPriceHistoryService.AddMealPriceHistory(meal.Id, meal.Price);

        return Result.Success();
    }

    public Result EditMeal(MealVm mealVM, int mealId)
    {
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
        {
            return Result.Failure(MealErrors.MealNotFound);
        }

        Meal oldMeal = _unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId)!;

        if (oldMeal.Price != mealVM.Price)
        {
            _mealPriceHistoryService.UpdateMealPriceHistory(mealId, mealVM.Price);
        }

        bool noNewData = ThereIsNewData(mealVM, oldMeal);

        if (noNewData)
        {
            mealVM.Image = oldMeal.Image;
            return Result.Failure(MealErrors.NoNewData);
        }

        bool isMealNameExists = _unitOfWorkRepository.Meals.IsExist(m => m.Name == mealVM.Name && m.Id != mealId);

        if (isMealNameExists)
        {
            return Result.Failure(MealErrors.DoublicatedMealName);
        }

        _mapper.Map(mealVM, oldMeal);

        _unitOfWorkRepository.Meals.Edit(oldMeal);      
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangeMealImage(IFormFile image, int mealId)
    {        
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
        {
            return Result.Failure(MealErrors.MealNotFound);
        }

        var uploadImageResult = _imageService.UploadImage(image);

        if (uploadImageResult.IsFailure)
        {
            return uploadImageResult;
        }

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

        var isOldImageDeleted = _imageService.DeleteImageIfExist(meal.Image);

        if (isOldImageDeleted.IsFailure)
        {
            _imageService.DeleteImageIfExist(uploadImageResult.Value);
            return isOldImageDeleted;
        }

        meal.Image = uploadImageResult.Value;

        _unitOfWorkRepository.Meals.ModifyProperty(meal, r => r.Image!);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    

    public Result<IQueryable<MealVm>> GetDeletedMeals(int restaurantId)
    {
        Result validateRestaurantIdResult = IsRestaurantIdExists(restaurantId);

        if (validateRestaurantIdResult.IsFailure)
        {
            return Result.Failure<IQueryable<MealVm>>(RestaurantErrors.InvalidRestaurantId);
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



    private Result IsRestaurantIdExists(int restaurantId)
    {
        if (restaurantId <= 0)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants.IsExist(expression: r => r.Id == restaurantId);

        if (!isRestaurantIdExists)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        return Result.Success();
    }

    private Result IsMealIdExists(int mealId)
    {
        if (mealId <= 0)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        bool isRestaurantIdExists = _unitOfWorkRepository.Meals.IsExist(expression: r => r.Id == mealId);

        if (!isRestaurantIdExists)
        {
            return Result.Failure(RestaurantErrors.InvalidRestaurantId);
        }

        return Result.Success();
    }

    private static bool ThereIsNewData(MealVm newMeal, Meal oldMeal)
    {
        return  oldMeal.Name == newMeal.Name && oldMeal.Description == newMeal.Description &&
                oldMeal.Price == newMeal.Price && oldMeal.NumberOfServings == newMeal.NumberOfServings &&
                oldMeal.IsAvailable == newMeal.IsAvailable && oldMeal.IsNewMeal == newMeal.IsNewMeal &&
                oldMeal.IsTrendingMeal == newMeal.IsTrendingMeal && oldMeal.Category == newMeal.Category;
    }

    private Meal FillMealData(MealVm mealVM, int restaurantId, string imageUrl)
    {
        Meal newMeal = new();

        _mapper.Map(mealVM, newMeal);

        newMeal.RestaurantId = restaurantId;
        newMeal.Image = imageUrl;

        return newMeal;
    }

    private Result<MealVm> MaptoMealVm(Meal meal)
    {
        MealVm mealVM = new();

        _mapper.Map(meal, mealVM);

        return Result.Success(mealVM);
    }
}
