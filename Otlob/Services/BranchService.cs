namespace Otlob.Services;

public class BranchService(IUnitOfWorkRepository unitOfWorkRepository, IRestaurantService restaurantService,
                           IDataProtectionProvider dataProtectionProvider) : IBranchService
{
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("BranchService");

    public IQueryable<BranchResponse> GetAllByRestaurantId(int restaurantId)
    {
        var branches = _unitOfWorkRepository.RestaurantBranches
            .GetAllWithSelect(
                expression: rb => rb.RestaurantId == restaurantId,
                tracked: false,
                selector: rb => new BranchResponse
                {
                    Key = _dataProtector.Protect(rb.Id.ToString()),
                    RestaurantKey = _dataProtector.Protect(rb.RestaurantId.ToString()),
                    Name = rb.Name,
                    Address = rb.Address,
                    DeliveryRadiusKm = rb.DeliveryRadiusKm,
                    MangerName = rb.MangerName,
                    MangerPhone = rb.MangerPhone,
                    LonCode = rb.Location.X,
                    LatCode = rb.Location.Y
                }
            )!;
        
        return branches;
    }

    public Result<BranchResponse> GetById(string id)
    {
        //TODO: Handle possible format exception
        int branchId = int.Parse(_dataProtector.Unprotect(id));

        var isBranchExist = IsBranchIdExists(branchId);
        
        if (isBranchExist.IsFailure)
        {
            return Result.Failure<BranchResponse>(BranchErrors.NotFound);
        }
        
        var branch = _unitOfWorkRepository.RestaurantBranches
            .GetOneWithSelect(
                expression: rb => rb.Id == branchId,
                tracked: false,
                selector: rb => new BranchResponse
                {
                    Key = _dataProtector.Protect(rb.Id.ToString()),
                    Name = rb.Name,
                    Address = rb.Address,
                    DeliveryRadiusKm = rb.DeliveryRadiusKm,
                    MangerName = rb.MangerName,
                    MangerPhone = rb.MangerPhone,
                    LonCode = rb.Location.X,
                    LatCode = rb.Location.Y
                }
            )!;

        return Result.Success(branch);
    }

    public Result Add(BranchRequest request, int restaurantId)
    {
        var isRestaurantExist = _restaurantService.IsRestaurantIdExists(restaurantId);

        if (isRestaurantExist.IsFailure)
        {
            return isRestaurantExist;
        }

        Result isValidBranchesCount = ValidBranchCount(restaurantId);
        
        if (isValidBranchesCount.IsFailure)
        {
            return isValidBranchesCount;
        }

        var isValidBranchData = ValidBranchDataForAdd(request, restaurantId);

        if (isValidBranchData.IsFailure)
        {
            return isValidBranchData;
        }

        RestaurantBranch restaurantBranch = new();

        request.MapToRestaurantBranch(restaurantBranch);
        restaurantBranch.RestaurantId = restaurantId;

        _unitOfWorkRepository.RestaurantBranches.Create(restaurantBranch);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    

    public Result Update(string id, BranchRequest request)
    {
        int branchId = int.Parse(_dataProtector.Unprotect(id));

        var isBranchExist = IsBranchIdExists(branchId);
        
        if (isBranchExist.IsFailure)
        {
            return isBranchExist;
        }

        var branch = _unitOfWorkRepository.RestaurantBranches
            .GetOne(expression: rb => rb.Id == branchId)!;

        var thereAreNewDataToUpdate = ThereAreNewDataToUpdate(request, branch);

        if (thereAreNewDataToUpdate.IsFailure)
        {
            return thereAreNewDataToUpdate;
        }

        var isValidBranchData = ValidBranchDataForUpdate(request, branch.RestaurantId, branchId);

        if (isValidBranchData.IsFailure)
        {
            return isValidBranchData;
        }

        request.MapToRestaurantBranch(branch);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public int GetRestaurantBranchesCountByRestaurantId(int restaurantId)
    {
        int totalBranches = _unitOfWorkRepository.RestaurantBranches
            .GetAllWithSelect(
                expression: rb => rb.RestaurantId == restaurantId,
                tracked: false,
                selector: rb => rb.Id
            )!
            .Count();

        return totalBranches;
    }

    public Result Delete(string key)
    {
        int branchId = int.Parse(_dataProtector.Unprotect(key));
        var isBranchExist = IsBranchIdExists(branchId);
        
        if (isBranchExist.IsFailure)
        {
            return isBranchExist;
        }

        _unitOfWorkRepository.RestaurantBranches.SoftDelete(rb => rb.Id == branchId);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }



    private Result ValidBranchCount(int restaurantId)
    {
        int restaurantBranches = _restaurantService.HowManyBranchesExistForRestaurant(restaurantId);
        int totalBranches = GetRestaurantBranchesCountByRestaurantId(restaurantId);

        if (totalBranches >= restaurantBranches)
        {
            return Result.Failure(BranchErrors.ExceedAllowedBranchesCount);
        }

        return Result.Success();
    }

    private Result IsBranchIdExists(int branchId)
    {
        var isExist = _unitOfWorkRepository.RestaurantBranches.IsExist(rb => rb.Id == branchId);
        
        if (!isExist)
        {
            return Result.Failure(BranchErrors.NotFound);
        }

        return Result.Success();
    }

    private Result ValidBranchDataForAdd(BranchRequest request, int restaurantId)
    {
        bool isAddressExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.Address == request.Address, ignoreQueryFilter: true);

        if (isAddressExists)
            return Result.Failure(BranchErrors.DoublicateddBranchAddress);

        bool isNameExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.Name == request.Name && rb.RestaurantId == restaurantId, ignoreQueryFilter: true);

        if (isNameExists)
            return Result.Failure(BranchErrors.DoublicatedBranchName);

        bool isManagerNameExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.MangerName == request.MangerName && rb.RestaurantId == restaurantId, ignoreQueryFilter: true);

        if (isManagerNameExists)
            return Result.Failure(BranchErrors.DoublicatedmanagerName);

        bool isManagerPhoneExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.MangerPhone == request.MangerPhone, ignoreQueryFilter: true);

        if (isManagerPhoneExists)
            return Result.Failure(BranchErrors.DoublicatedManagedPhoneNumber);

        return Result.Success();
    }

    private Result ValidBranchDataForUpdate(BranchRequest request, int restaurantId, int branchId)
    {
        bool isAddressExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.Address == request.Address && rb.Id != branchId, ignoreQueryFilter: true);

        if (isAddressExists)
            return Result.Failure(BranchErrors.DoublicateddBranchAddress);

        bool isNameExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.Name == request.Name && rb.RestaurantId == restaurantId && rb.Id != branchId, ignoreQueryFilter: true);

        if (isNameExists)
            return Result.Failure(BranchErrors.DoublicatedBranchName);

        bool isManagerNameExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.MangerName == request.MangerName && rb.RestaurantId == restaurantId && rb.Id != branchId, ignoreQueryFilter: true);

        if (isManagerNameExists)
            return Result.Failure(BranchErrors.DoublicatedmanagerName);

        bool isManagerPhoneExists = _unitOfWorkRepository.RestaurantBranches
            .IsExist(rb => rb.MangerPhone == request.MangerPhone && rb.Id != branchId, ignoreQueryFilter: true);

        if (isManagerPhoneExists)
            return Result.Failure(BranchErrors.DoublicatedManagedPhoneNumber);

        return Result.Success();
    }

    private static Result ThereAreNewDataToUpdate(BranchRequest request, RestaurantBranch branch)
    {
        if (branch.Name == request.Name &&
            branch.Address == request.Address &&
            branch.DeliveryRadiusKm == request.DeliveryRadiusKm &&
            branch.MangerName == request.MangerName &&
            branch.MangerPhone == request.MangerPhone &&
            branch.Location.X == request.LonCode &&
            branch.Location.Y == request.LatCode)
        {
            return Result.Failure(BranchErrors.NoNewDataToUpdate);
        }

        return Result.Success();
    }
}
