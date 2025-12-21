namespace Otlob.Services;

public class CommercialRegistrationService(IUnitOfWorkRepository unitOfWorkRepository, IFileService fileService,
                                           IHttpContextAccessor httpContextAccessor, IMapper mapper, 
                                           IRestaurantProgressStatus restaurantProgressStatus, IDataProtectionProvider dataProtectionProvider) : ICommercialRegistrationService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantProgressStatus _restaurantProgressStatus = restaurantProgressStatus;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<CommercialRegistrationResponse> GetCommercialRegistration(string id)
    {
        //TODO: Handle exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        bool isRestaurantHasCertificate = IsRestaurantHasCertificate(restaurantId);

        if (!isRestaurantHasCertificate)
        {
            return Result.Failure<CommercialRegistrationResponse>(CommertialRegistrationErrors.NotFoundCertificate);
        }

        var response = _unitOfWorkRepository.CommercialRegistrations
            .GetOneWithSelect(
                expression: cr => cr.RestaurantId == restaurantId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: cr => new CommercialRegistrationResponse
                {
                    Id = _dataProtector.Protect(cr.Id.ToString()),
                    RestaurantId = id,
                    RegistrationNumber = cr.RegistrationNumber,
                    DateOfIssuance = cr.DateOfIssuance,
                    ExpiryDate = cr.ExpiryDate,
                    DocumentStatus = cr.Status,
                    CertificateRegistration = new FileResponse
                    {
                        Id = cr.CertificateRegistrationId,
                        FileName = cr.CertificateRegistration.FileName,
                        StoredFileName = cr.CertificateRegistration.StoredFileName,
                        ContetntType = cr.CertificateRegistration.ContetntType
                    }
                }
            );

        return Result.Success(response!);
    }   

    public async Task<Result> UploadAsync(CommercialRegistrationRequest request, UploadFileRequest fileRequest)
    {

        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        ProgressStatus restauratnPrgressStatus = _restaurantProgressStatus.GetRestaurantProgressStatus(restaurantId);

        if (restauratnPrgressStatus != ProgressStatus.RestaurantProfileCompleted)
        {
            return Result.Failure(CommertialRegistrationErrors.InvalidProgressStatus);
        }

        bool validCertificateNumber = _unitOfWorkRepository.CommercialRegistrations
            .IsExist(cr => cr.RegistrationNumber == request.RegistrationNumber);

        if (validCertificateNumber)
        {
            return Result.Failure(CommertialRegistrationErrors.DoublicatedRegistrationNumber);
        }        

        var certificateId = await _fileService.UploadFileAsync(fileRequest.File);

        var commercialRegistration = _mapper.Map<CommercialRegistration>((request, restaurantId, certificateId));

        _unitOfWorkRepository.CommercialRegistrations.Add(commercialRegistration);

        _restaurantProgressStatus.ChangeRestaurantProgressStatus(restaurantId, ProgressStatus.CommercialRegistrationCompleted);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangCommercialRegistrationStatus(string id, DocumentStatus status)
    {
        int commercialRegistrationId = int.Parse(_dataProtector.Unprotect(id));

        var certificateExistResult = IsCertificateExist(commercialRegistrationId);

        if (certificateExistResult.IsFailure)
        {
            return certificateExistResult;
        }

        var commercialRegistration = _unitOfWorkRepository.CommercialRegistrations
            .GetOneWithSelect
            (
                expression: cr => cr.Id == commercialRegistrationId,
                selector: cr => new CommercialRegistration
                {
                    Id = cr.Id,
                    Status = cr.Status
                }
            )!;

        commercialRegistration.Status = status;

        _unitOfWorkRepository.CommercialRegistrations.ModifyProperty(commercialRegistration, cr => cr.Status);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public bool IsRestaurantHasCertificate(int restaurantId) =>
         _unitOfWorkRepository.CommercialRegistrations.IsExist(cr => cr.RestaurantId == restaurantId);


    private Result IsCertificateExist(int id)
    {
        bool isExist = _unitOfWorkRepository.CommercialRegistrations
            .IsExist(cr => cr.Id == id);

        if (!isExist)
        {
            return Result.Failure(CommertialRegistrationErrors.NotFoundCertificate);

        }

        return Result.Success();
    }
}
