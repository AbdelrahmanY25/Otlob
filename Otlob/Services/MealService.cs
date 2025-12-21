namespace Otlob.Services;

public class MealService(IMapper mapper, IFileService imageService,
                         IUnitOfWorkRepository unitOfWorkRepository,
                         IRestaurantService restaurantService,
                         IEncryptionService encryptionService,
                         IMealPriceHistoryService mealPriceHistoryService, 
                         IMealOptionGroupService optionGroupService) : IMealService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _imageService = imageService;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IMealOptionGroupService _optionGroupService = optionGroupService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IMealPriceHistoryService _mealPriceHistoryService = mealPriceHistoryService;

    public Result<IQueryable<MealResponse>>? GetAllByRestaurantId(int restaurantlId)
    {
        var response = _unitOfWorkRepository.Meals
            .GetAllWithSelect
             (
                expression: m => m.RestaurantId == restaurantlId,
                tracked: false,
                selector: m => new MealResponse
                {
                    Key = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Image = m.Image,
                }
             )!;

        if (response is null)
            return Result.Failure<IQueryable<MealResponse>>(RestaurantErrors.NotFound);

        return Result.Success(response);
    }

    public Result<IQueryable<MealResponse>>? GetAllByCategoryId(int categoryId)
    {
        var response = _unitOfWorkRepository.Meals
            .GetAllWithSelect
             (
                expression: m => m.CategoryId == categoryId,
                tracked: false,
                selector: m => new MealResponse
                {
                    Key = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Image = m.Image,
                }
             )!;

        if (response is null)
            return Result.Failure<IQueryable<MealResponse>>(MealCategoriesErrors.NotFound);


        return Result.Success(response);
    }   

    public Result<MealResponse> GetForUpdate(string mealId)
    {        
        var meal = _unitOfWorkRepository.Meals
            .GetOne(expression: m => m.Id == mealId, tracked: false, ignoreQueryFilter: true);

        if (meal is null)
            return Result.Failure<MealResponse>(MealErrors.MealNotFound);
        
        var response = FillResponseData(meal!);

        return Result.Success(response);
    }

    public async Task<Result> AddAsync(int restaurantId, MealRequest request, UploadImageRequest imageRequest)
    {
        // TODO: Handle Exception
        int categoryId = _encryptionService.Decrypt(request.SelectedCategoryKey);

        var result = ValidateMealOnAdd(restaurantId, categoryId, request);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        var isImageUploaded = _imageService.UploadImage(imageRequest.Image)!;

        if (isImageUploaded.IsFailure)
            return Result.Failure(isImageUploaded.Error);

        Meal meal = FillMealData(request, restaurantId, categoryId, isImageUploaded.Value);


        // Add meal option groups and items if exist
        if (request.HasOptionGroup && request.OptionGroups is not null)
        {
            var addOptionsResult = await _optionGroupService.Add(request.OptionGroups, meal.Id);
            
            if (addOptionsResult.IsFailure)
                return Result.Failure(addOptionsResult.Error);
        }

        // add AddOns if exist

        _mealPriceHistoryService.AddMealPriceHistory(meal.Id, meal.Price);

        _unitOfWorkRepository.Meals.Add(meal);  
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(MealRequest request, string mealId, int restaurantId)
    {
        // TODO: Handle Exception
        int categoryId = _encryptionService.Decrypt(request.SelectedCategoryKey);

        var validMeal = ValidateMealOnUpdate(restaurantId, mealId, categoryId, request);

        if (validMeal.IsFailure)
            return Result.Failure(validMeal.Error);

        Meal oldMeal = _unitOfWorkRepository.Meals.GetOne(expression: m => m.Id == mealId)!;       

        // update meal options and items
        if ((!request.HasOptionGroup && oldMeal.HasOptionGroup) ||
            (request.HasOptionGroup && !oldMeal.HasOptionGroup) ||
            (request.HasOptionGroup && oldMeal.HasOptionGroup))
        {
            var updateOptionsResult = await _optionGroupService.Update(request.OptionGroups!, mealId);
            
            if (updateOptionsResult.IsFailure)
                return Result.Failure(updateOptionsResult.Error);
        }

        // update AddOns if exist

        if (oldMeal.Price != request.Price)
            _mealPriceHistoryService.UpdateMealPriceHistory(mealId, request.Price);
        
        _mapper.Map(request, oldMeal);
        oldMeal.CategoryId = categoryId;

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result UpdateMealImage(UploadImageRequest imageRequest, string mealId)
    {
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
                return Result.Failure(MealErrors.MealNotFound);        

        var uploadImageResult = _imageService.UploadImage(imageRequest.Image);

        if (uploadImageResult.IsFailure)
                return uploadImageResult;

        Meal meal = GetMealImageById(mealId);

        _imageService.DeleteImage(meal.Image);

        meal.Image = uploadImageResult.Value;

        _unitOfWorkRepository.Meals.ModifyProperty(meal, r => r.Image!);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    

    public Result<IQueryable<MealResponse>> GetDeletedMeals(int restaurantId)
    {
        var response = _unitOfWorkRepository.Meals
            .GetAllWithSelect
             (
                expression: m => m.RestaurantId == restaurantId && EFCore.Property<bool>(m, "IsDeleted"),
                tracked: false,
                ignoreQueryFilter: true,
                selector: m => new MealResponse
                {
                    Key = m.Id,
                    Name = m.Name,
                    Image = m.Image,
                    Price = m.Price,
                }
             )!;

        if (response is null)
            return Result.Failure<IQueryable<MealResponse>>(RestaurantErrors.NotFound);


        return Result.Success(response);
    }

    public Result DeleteMeal(string mealId)
    {
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
            return Result.Failure(MealErrors.MealNotFound);

        var mealOptionGroupIds = _unitOfWorkRepository.MealOptionGroups
            .GetAllWithSelect
             (
                expression: og => og.MealId == mealId,
                tracked: false,
                selector: og => og.MealOptionGroupId
             )!
             .ToList();
        
        if (mealOptionGroupIds.Count > 0)
        {
            _unitOfWorkRepository.MealOptionItems.SoftDelete(expression: oi => mealOptionGroupIds.Contains(oi.OptionGroupId));
            _unitOfWorkRepository.MealOptionGroups.SoftDelete(expression: og => og.MealId == mealId);
        }

        _unitOfWorkRepository.Meals.SoftDelete(expression: m => m.Id == mealId);
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result UnDeleteMeal(string mealId)
    {        
        Result validateMealIdResult = IsMealIdExists(mealId);

        if (validateMealIdResult.IsFailure)
            return Result.Failure(MealErrors.MealNotFound);

        var mealOptionGroupIds = _unitOfWorkRepository.MealOptionGroups
            .GetAllWithSelect
             (
                expression: og => og.MealId == mealId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: og => og.MealOptionGroupId
             )!
             .ToList();

        if (mealOptionGroupIds.Count > 0)
        {
            _unitOfWorkRepository.MealOptionItems.UnSoftDelete(expression: oi => mealOptionGroupIds.Contains(oi.OptionGroupId));
            _unitOfWorkRepository.MealOptionGroups.UnSoftDelete(expression: og => og.MealId == mealId);
        }

        _unitOfWorkRepository.Meals.UnSoftDelete(expression: m => m.Id == mealId);
        
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }



    private Meal GetMealImageById(string mealId)
    {
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

        return meal;
    }
    
    private Result ValidateMealOnAdd(int restaurantId, int categoryId, MealRequest request)
    {
        Result isRestaurantIdExists = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (isRestaurantIdExists.IsFailure)
            return Result.Failure(RestaurantErrors.NotFound);

        bool isCategoryIdExists = _unitOfWorkRepository.MealCategories.IsExist(mc => mc.Id == categoryId);

        if (!isCategoryIdExists)
            return Result.Failure(MealCategoriesErrors.NotFound);

        bool isMealNameExists = _unitOfWorkRepository.Meals
            .IsExist(m => m.RestaurantId == restaurantId && m.Name == request.Name, ignoreQueryFilter: true);

        if (isMealNameExists)
            return Result.Failure(MealErrors.DoublicatedMealName);

        return Result.Success();
    }

    private Result ValidateMealOnUpdate(int restaurantId, string mealId, int categoryId, MealRequest request)
    {
        Result isMealIdExists = IsMealIdExists(mealId);

        if (isMealIdExists.IsFailure)
            return Result.Failure(MealErrors.MealNotFound);

        bool isCategoryIdExists = _unitOfWorkRepository.MealCategories.IsExist(mc => mc.Id == categoryId);

        if (!isCategoryIdExists)
            return Result.Failure(MealCategoriesErrors.NotFound);

        bool isMealNameExists = _unitOfWorkRepository.Meals
            .IsExist(m => m.RestaurantId == restaurantId && m.Name == request.Name && m.Id != mealId, ignoreQueryFilter: true);

        if (isMealNameExists)
            return Result.Failure(MealErrors.DoublicatedMealName);

        return Result.Success();
    }

    private Result IsMealIdExists(string mealId)
    {
        bool ismealIdExists = _unitOfWorkRepository.Meals.IsExist(expression: r => r.Id == mealId, ignoreQueryFilter: true);

        if (!ismealIdExists)
            return Result.Failure(RestaurantErrors.NotFound);

        return Result.Success();
    }

    private static bool ThereIsNewData(MealRequest request, Meal oldMeal, int categoryId)
    {
        return  oldMeal.Name == request.Name && oldMeal.Description == request.Description &&
                oldMeal.Price == request.Price && oldMeal.IsAvailable == request.IsAvailable && 
                oldMeal.IsNewMeal == request.IsNewMeal && oldMeal.IsTrendingMeal == request.IsTrendingMeal &&
                oldMeal.CategoryId == categoryId && oldMeal.NumberOfServings == request.NumberOfServings;
    }

    private Meal FillMealData(MealRequest request, int restaurantId, int categoryId, string imageUrl)
    {
        Meal meal = _mapper.Map<Meal>(request);

        meal.RestaurantId = restaurantId;
        meal.CategoryId = categoryId;
        meal.Image = imageUrl;

        return meal;
    }

    private MealResponse FillResponseData(Meal meal)
    {
        var response = _mapper.Map<MealResponse>(meal);
        response.CategoryKey = _encryptionService.Encrypt(meal!.CategoryId);

        response.Categories = _unitOfWorkRepository.MealCategories
            .GetAllWithSelect
             (
                expression: mc => mc.RestaurantId == meal.RestaurantId,
                tracked: false,
                selector: mc => new MenuCategoryResponse
                {
                    Key = _encryptionService.Encrypt(mc.Id),
                    Name = mc.Name
                }
             );

        response.OptionGroups = _unitOfWorkRepository.MealOptionGroups
            .GetAllWithSelect
             (
                expression: og => og.MealId == meal.Id,
                tracked: false,
                selector: og => new OptionGroupResponse
                {
                    Id = og.MealOptionGroupId,
                    Name = og.Name,
                    DisplayOrder = og.DisplayOrder,
                    OptionItems = og.OptionItems.Select(oi => new OptionItemResponse
                    {
                        Id = oi.MealOptionItemId,
                        Name = oi.Name,
                        Price = oi.Price,
                        Image = oi.Image,
                        DisplayOrder = oi.DisplayOrder,
                        IsPobular = oi.IsPobular
                    })                    
                }
             )!;

        if (response.OptionGroups.Any())
        {
            response.OptionGroups = response.OptionGroups
                .Select(og =>
                {
                    og.OptionItems = og.OptionItems.OrderBy(oi => oi.DisplayOrder);
                    return og;
                })
                .OrderBy(og => og.DisplayOrder);
        }

        return response;
    }
}
