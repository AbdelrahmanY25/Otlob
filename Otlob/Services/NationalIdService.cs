namespace Otlob.Services;

public class NationalIdService(IUnitOfWorkRepository unitOfWorkRepository, IHttpContextAccessor httpContextAccessor,
    IFileService fileService, IMapper mapper, IRestaurantProgressStatus restaurantProgressStatus,
    IDataProtectionProvider dataProtectionProvider) : INationalIdService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantProgressStatus _restaurantProgressStatus = restaurantProgressStatus;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<NationalIdResponse> GetNationalId(string id)
    {
        //TODO: Handle exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        bool isRestaurantHasCertificate = IsRestaurantUploadHisNationalIdCard(restaurantId);

        if (!isRestaurantHasCertificate)
        {
            return Result.Failure<NationalIdResponse>(NationalIdErrors.NotFoundNationalIdCard);
        }

        var response = _unitOfWorkRepository.NationalIds
            .GetOneWithSelect(
                expression: cr => cr.RestaurantId == restaurantId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: nid => new NationalIdResponse
                {
                    Id = _dataProtector.Protect(nid.Id.ToString()),
                    RestaurantId = id,
                    NationalIdNumber = nid.NationalIdNumber,
                    FullName = nid.FullName,
                    NationalIdExpiryDate = nid.NationalIdExpiryDate,
                    Status = nid.Status,
                    NationalIdCard = new FileResponse
                    {
                        Id = nid.NationalCardId,
                        FileName = nid.NationalCard.FileName,
                        StoredFileName = nid.NationalCard.StoredFileName,
                        ContetntType = nid.NationalCard.ContetntType
                    },
                    SignatureImage = new FileResponse
                    {
                        Id = nid.SignatureImageId,
                        FileName = nid.SignatureImage.FileName,
                        StoredFileName = nid.SignatureImage.StoredFileName,
                        ContetntType = nid.SignatureImage.ContetntType
                    }
                }
            );

        return Result.Success(response!);
    }

    public async Task<Result> UploadAsync(NationalIdRequest request, UploadFileRequest nationalIdCard, UploadImageRequest signature)
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        ProgressStatus progressStatus = _restaurantProgressStatus.GetRestaurantProgressStatus(restaurantId);

        if (progressStatus != ProgressStatus.BankAccountCompleted)
        {
            return Result.Failure(NationalIdErrors.InvalidProgressStatus);
        }

        var validateResult = ValidateNationalId(request);

        if (validateResult.IsFailure)
        {
            return validateResult;
        }

        string nationalcardId = await _fileService.UploadFileAsync(nationalIdCard.File);

        string signatureId = await _fileService.UploadFileAsync(signature.Image);

        NationalId nationalId = new()
        {
            NationalIdNumber = request.NationalIdNumber,
            FullName = request.FullName,
            NationalIdExpiryDate = request.NationalIdExpiryDate,
            NationalCardId = nationalcardId,
            SignatureImageId = signatureId,
            RestaurantId = restaurantId
        };

        _unitOfWorkRepository.NationalIds.Add(nationalId);

        _restaurantProgressStatus.ChangeRestaurantProgressStatus(restaurantId, ProgressStatus.NationalIdCompleted);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangNationalIdStatus(string id, DocumentStatus status)
    {
        int nationalIdId = int.Parse(_dataProtector.Unprotect(id));

        var certificateExistResult = IsCertificateExist(nationalIdId);

        if (certificateExistResult.IsFailure)
        {
            return certificateExistResult;
        }

        var nationalId = _unitOfWorkRepository.NationalIds
            .GetOneWithSelect
            (
                expression: cr => cr.Id == nationalIdId,
                selector: cr => new NationalId
                {
                    Id = cr.Id,
                    Status = cr.Status
                }
            )!;

        nationalId.Status = status;

        _unitOfWorkRepository.NationalIds.ModifyProperty(nationalId, cr => cr.Status);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public bool IsRestaurantUploadHisNationalIdCard(int restaurantId) =>
         _unitOfWorkRepository.NationalIds.IsExist(cr => cr.RestaurantId == restaurantId);

    private Result ValidateNationalId(NationalIdRequest request)
    {
        bool isValidIdNumber = _unitOfWorkRepository.NationalIds
            .IsExist(nId => nId.NationalIdNumber == request.NationalIdNumber);

        if (isValidIdNumber)
        {
            return Result.Failure(NationalIdErrors.DoublicatedNationalIdNumber);
        }

        return Result.Success();
    }

    private Result IsCertificateExist(int id)
    {
        bool isExist = _unitOfWorkRepository.NationalIds
            .IsExist(cr => cr.Id == id);

        if (!isExist)
        {
            return Result.Failure(NationalIdErrors.NotFoundNationalIdCard);

        }

        return Result.Success();
    }
}
