namespace Otlob.Services;

public class MealAddOnService(IUnitOfWorkRepository unitOfWorkRepository, IFileService fileService, IMapper mapper) : IMealAddOnService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Result<IQueryable<AddOnResponse>> GetAllByRestaurantId(int restaurantId)
    {
        var response = _unitOfWorkRepository.MealAddOns
            .Get(expression: ao => ao.RestaurantId == restaurantId)!
            .Select(r => new AddOnResponse
            {
                Key = r.Id,
                Name = r.Name,
                Price = r.Price,
                Image = r.Image
            });

        if (response is null)
            return Result.Failure<IQueryable<AddOnResponse>>(MealAddOnErrors.NotFound);


        return Result.Success(response);
    }

    public Result Add(AddOnRequest request, int restaurantId)
    {       
        var validationResult = ValidateOnAdd(request, restaurantId);
        
        if (validationResult.IsFailure)
            return validationResult;

        if (request.ImageRequest is not null)
        {
            var imagePath = _fileService.UploadImage(request.ImageRequest.Image);
            
            var addOn = _mapper.Map<MealAddOn>((restaurantId, request, imagePath.Value));
            
            _unitOfWorkRepository.MealAddOns.Add(addOn);
        }
        else
        {
            var addOn = _mapper.Map<MealAddOn>((restaurantId, request, string.Empty));
            
            _unitOfWorkRepository.MealAddOns.Add(addOn);
        }
                
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result Update(AddOnRequest request, string id)
    {
        var addOn = _unitOfWorkRepository.MealAddOns.GetOne(expression: ma => ma.Id == id)!;

        var validationResult = ValidateOnUpdate(addOn, request);

        if (validationResult.IsFailure)
            return validationResult;

        if (addOn.Name == request.Name && addOn.Price == request.Price && request.ImageRequest is null)
            return Result.Success();

        addOn.Name = request.Name;        
        addOn.Price = request.Price;
        
        if (request.ImageRequest is not null)
        {
            _fileService.DeleteImage(addOn.Image);
            var imagePath = _fileService.UploadImage(request.ImageRequest.Image);
            
            addOn.Image = imagePath.Value;
        } 
                
        _unitOfWorkRepository.SaveChanges();
        
        return Result.Success();
    }

    public Result Delete(string id)
    {
        var addOn = _unitOfWorkRepository.MealAddOns.GetOne(expression: ma => ma.Id == id)!;

        if (addOn is null)
            return Result.Failure(RestaurantErrors.NotFound);

        _unitOfWorkRepository.MealAddOns.SoftDelete(ao => ao.Id == id);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }



    private Result ValidateOnAdd(AddOnRequest request, int restaurantId)
    {
        bool isRestaurantIdExists = _unitOfWorkRepository.Restaurants.IsExist(r => r.Id == restaurantId);

        if (!isRestaurantIdExists)
            return Result.Failure(RestaurantErrors.NotFound);

        bool isDuplicateNameExists = _unitOfWorkRepository.MealAddOns
            .IsExist(ma => ma.Name == request.Name && ma.RestaurantId == restaurantId);

        if (isDuplicateNameExists)
            return Result.Failure(MealAddOnErrors.DuplicateName);

        return Result.Success();
    }

    private Result ValidateOnUpdate(MealAddOn addOn, AddOnRequest request)
    {
        if (addOn is null)
            return Result.Failure(MealAddOnErrors.NotFound);

        bool isDuplicateNameExists = _unitOfWorkRepository.MealAddOns
            .IsExist(ma => ma.Name == request.Name && ma.RestaurantId == addOn.RestaurantId && ma.Id != addOn.Id);

        if (isDuplicateNameExists)
            return Result.Failure(MealAddOnErrors.DuplicateName);

        return Result.Success();
    }
}
