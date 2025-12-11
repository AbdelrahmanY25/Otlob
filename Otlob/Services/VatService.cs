namespace Otlob.Services;

public class VatService(IUnitOfWorkRepository unitOfWorkRepository, IFileService fileService,
                        IHttpContextAccessor httpContextAccessor, IMapper mapper, IRestaurantProgressStatus restaurantProgressStatus,
                        IDataProtectionProvider dataProtectionProvider) : IVatService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantProgressStatus _restaurantProgressStatus = restaurantProgressStatus;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<VatResponse> GetVat(string id)
    {
        //TODO: Handle exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        bool isRestaurantHasCertificate = IsRestaurantHasVatCertificate(restaurantId);

        if (!isRestaurantHasCertificate)
        {
            return Result.Failure<VatResponse>(VatErrors.NotFoundCertificate);
        }

        var response = _unitOfWorkRepository.Vats
            .GetOneWithSelect(
                expression: cr => cr.RestaurantId == restaurantId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: v => new VatResponse
                {
                    Id = _dataProtector.Protect(v.Id.ToString()),
                    RestaurantId = id,
                    VatNumber = v.VatNumber,
                    DocumentStatus = v.Status,
                    VatCertificate = new FileResponse
                    {
                        Id = v.VatCertificateId,
                        FileName = v.VatCertificate.FileName,
                        StoredFileName = v.VatCertificate.StoredFileName,
                        ContetntType = v.VatCertificate.ContetntType
                    }
                }
            );

        return Result.Success(response!);
    }

    public async Task<Result> UploadAsync(VatRequest request, UploadFileRequest fileRequest)
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        ProgressStatus progressStatus = _restaurantProgressStatus.GetRestaurantProgressStatus(restaurantId);

        if (progressStatus != ProgressStatus.TradeMarkCompleted)
        {
            return Result.Failure(VatErrors.InvalidProgressStatus);
        }       

        bool validVatNumber = _unitOfWorkRepository.Vats
            .IsExist(tm => tm.VatNumber == request.VatNumber);

        if (validVatNumber)
        {
            return Result.Failure(VatErrors.DoublicatedVatNumber);
        }

        var certificateId = await _fileService.UploadFileAsync(fileRequest.File);

        var vat = _mapper.Map<VAT>((request, restaurantId, certificateId));

        _unitOfWorkRepository.Vats.Create(vat);

        _restaurantProgressStatus.ChangeRestaurantProgressStatus(restaurantId, ProgressStatus.VatCertificateCompleted);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangVatStatus(string id, DocumentStatus status)
    {
        int vatId = int.Parse(_dataProtector.Unprotect(id));

        var certificateExistResult = IsCertificateExist(vatId);

        if (certificateExistResult.IsFailure)
        {
            return certificateExistResult;
        }

        var vat = _unitOfWorkRepository.Vats
            .GetOneWithSelect
            (
                expression: cr => cr.Id == vatId,
                selector: cr => new VAT
                {
                    Id = cr.Id,
                    Status = cr.Status
                }
            )!;

        vat.Status = status;

        _unitOfWorkRepository.Vats.ModifyProperty(vat, cr => cr.Status);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public bool IsRestaurantHasVatCertificate(int restaurantId) =>
         _unitOfWorkRepository.Vats.IsExist(cr => cr.RestaurantId == restaurantId);

    private Result IsCertificateExist(int id)
    {
        bool isExist = _unitOfWorkRepository.Vats
            .IsExist(cr => cr.Id == id);

        if (!isExist)
        {
            return Result.Failure(VatErrors.NotFoundCertificate);

        }

        return Result.Success();
    }
}
