namespace Otlob.Services;

public class TradeMarkService(IUnitOfWorkRepository unitOfWorkRepository, IFileService fileService,
                              IHttpContextAccessor httpContextAccessor, IMapper mapper, IRestaurantProgressStatus restaurantProgressStatus,
                              IDataProtectionProvider dataProtectionProvider) : ITradeMarkService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantProgressStatus _restaurantProgressStatus = restaurantProgressStatus;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public Result<TradeMarkResponse> GetTradeMark(string id)
    {
        //TODO: Handle exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        bool isRestaurantHasCertificate = IsRestaurantHasTradeMarkCertificate(restaurantId);

        if (!isRestaurantHasCertificate)
        {
            return Result.Failure<TradeMarkResponse>(TradeMarkErrors.NotFoundCertificate);
        }

        var response = _unitOfWorkRepository.TradeMarks
            .GetOneWithSelect(
                expression: cr => cr.RestaurantId == restaurantId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: tm => new TradeMarkResponse
                {
                    Id = _dataProtector.Protect(tm.Id.ToString()),
                    RestaurantId = id,
                    TrademarkName = tm.TrademarkName,
                    TrademarkNumber = tm.TrademarkNumber,
                    ExpiryDate = tm.ExpiryDate,
                    DocumentStatus = tm.Status,
                    TradeMarkCertificate = new FileResponse
                    {
                        Id = tm.TradeMarkCertificateId,
                        FileName = tm.TradeMarkCertificate.FileName,
                        StoredFileName = tm.TradeMarkCertificate.StoredFileName,
                        ContetntType = tm.TradeMarkCertificate.ContetntType
                    }
                }
            );

        return Result.Success(response!);
    }

    public async Task<Result> UploadAsync(TradeMarkRequest request, UploadFileRequest fileRequest)
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        ProgressStatus progressStatus = _restaurantProgressStatus.GetRestaurantProgressStatus(restaurantId);

        if (progressStatus != ProgressStatus.CommercialRegistrationCompleted)
        {
            return Result.Failure(TradeMarkErrors.InvalidProgressStatus);
        }

        bool validTradeMarkName = _unitOfWorkRepository.TradeMarks
            .IsExist(tm => tm.TrademarkName == request.TrademarkName);

        if (validTradeMarkName)
        {
            return Result.Failure(TradeMarkErrors.DoublicatedTradeMarkName);
        }

        bool validTradeMarkNumber = _unitOfWorkRepository.TradeMarks
            .IsExist(tm => tm.TrademarkNumber == request.TrademarkNumber);

        if (validTradeMarkNumber)
        {
            return Result.Failure(TradeMarkErrors.DoublicatedTradeMarkNumber);
        }        

        var certificateId = await _fileService.UploadFileAsync(fileRequest.File);

        var tradeMark = _mapper.Map<TradeMark>((request, restaurantId, certificateId));

        _unitOfWorkRepository.TradeMarks.Create(tradeMark);

        _restaurantProgressStatus.ChangeRestaurantProgressStatus(restaurantId, ProgressStatus.TradeMarkCompleted);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangTradeMarkStatus(string id, DocumentStatus status)
    {
        int tradeMarkId = int.Parse(_dataProtector.Unprotect(id));

        var certificateExistResult = IsCertificateExist(tradeMarkId);

        if (certificateExistResult.IsFailure)
        {
            return certificateExistResult;
        }

        var tradeMark = _unitOfWorkRepository.TradeMarks
            .GetOneWithSelect
            (
                expression: cr => cr.Id == tradeMarkId,
                selector: cr => new TradeMark
                {
                    Id = cr.Id,
                    Status = cr.Status
                }
            )!;

        tradeMark.Status = status;

        _unitOfWorkRepository.TradeMarks.ModifyProperty(tradeMark, cr => cr.Status);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public bool IsRestaurantHasTradeMarkCertificate(int restaurantId) =>
         _unitOfWorkRepository.TradeMarks.IsExist(cr => cr.RestaurantId == restaurantId);


    private Result IsCertificateExist(int id)
    {
        bool isExist = _unitOfWorkRepository.TradeMarks
            .IsExist(cr => cr.Id == id);

        if (!isExist)
        {
            return Result.Failure(TradeMarkErrors.NotFoundCertificate);

        }

        return Result.Success();
    }
}
