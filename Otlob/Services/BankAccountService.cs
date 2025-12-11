namespace Otlob.Services;

public class BankAccountService(IUnitOfWorkRepository unitOfWorkRepository, IFileService fileService,
                                IHttpContextAccessor httpContextAccessor, IMapper mapper, IRestaurantProgressStatus restaurantProgressStatus,
                                IDataProtectionProvider dataProtectionProvider) : IBankAccountService
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileService _fileService = fileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IRestaurantProgressStatus _restaurantProgressStatus = restaurantProgressStatus;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");


    public Result<BankAccountResponse> GetBankAccount(string id)
    {
        //TODO: Handle exception for unprotect
        int restaurantId = int.Parse(_dataProtector.Unprotect(id));

        bool isRestaurantHasCertificate = IsRestaurantHasBankAccountCertificate(restaurantId);

        if (!isRestaurantHasCertificate)
        {
            return Result.Failure<BankAccountResponse>(BankAccountErrors.NotFoundCertificate);
        }

        var response = _unitOfWorkRepository.BankAccounts
            .GetOneWithSelect(
                expression: cr => cr.RestaurantId == restaurantId,
                tracked: false,
                ignoreQueryFilter: true,
                selector: ba => new BankAccountResponse
                {
                    Id = _dataProtector.Protect(ba.Id.ToString()),
                    RestaurantId = id,
                    AccountHolderName = ba.AccountHolderName,
                    AccountType = ba.AccountType,
                    AccountNumber = ba.AccountNumber,
                    BankName = ba.BankName,
                    Iban = ba.Iban,
                    BankCertificateIssueDate = ba.BankCertificateIssueDate,
                    Status = ba.Status,

                    BankAccountCertificate = new FileResponse
                    {
                        Id = ba.BankCertificateId,
                        FileName = ba.BankCertificate.FileName,
                        StoredFileName = ba.BankCertificate.StoredFileName,
                        ContetntType = ba.BankCertificate.ContetntType
                    }
                }
            );

        return Result.Success(response!);
    }

    public async Task<Result> UploadAsync(BankAccountRequest request, UploadFileRequest fileRequest)
    {
        int restaurantId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(StaticData.RestaurantId)!);

        ProgressStatus progressStatus = _restaurantProgressStatus.GetRestaurantProgressStatus(restaurantId);

        if (progressStatus != ProgressStatus.VatCertificateCompleted)
        {
            return Result.Failure(BankAccountErrors.InvalidProgressStatus);
        }

        var validBanckAccountResult = ValidBanckAccount(request);

        if (validBanckAccountResult.IsFailure)
        {
            return validBanckAccountResult;
        }

        var certificateId = await _fileService.UploadFileAsync(fileRequest.File);

        var banckAccount = _mapper.Map<BankAccount>((request, restaurantId, certificateId));

        _unitOfWorkRepository.BankAccounts.Create(banckAccount);

        _restaurantProgressStatus.ChangeRestaurantProgressStatus(restaurantId, ProgressStatus.BankAccountCompleted);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public Result ChangBankAccountStatus(string id, DocumentStatus status)
    {
        int bankAccountId = int.Parse(_dataProtector.Unprotect(id));

        var certificateExistResult = IsCertificateExist(bankAccountId);

        if (certificateExistResult.IsFailure)
        {
            return certificateExistResult;
        }

        var bankAccount = _unitOfWorkRepository.BankAccounts
            .GetOneWithSelect
            (
                expression: cr => cr.Id == bankAccountId,
                selector: cr => new BankAccount
                {
                    Id = cr.Id,
                    Status = cr.Status
                }
            )!;

        bankAccount.Status = status;

        _unitOfWorkRepository.BankAccounts.ModifyProperty(bankAccount, cr => cr.Status);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public bool IsRestaurantHasBankAccountCertificate(int restaurantId) =>
         _unitOfWorkRepository.BankAccounts.IsExist(cr => cr.RestaurantId == restaurantId);

    private Result ValidBanckAccount(BankAccountRequest request)
    {
        bool validAccountNumber = _unitOfWorkRepository.BankAccounts
            .IsExist(tm => tm.AccountNumber == request.AccountNumber);

        if (validAccountNumber)
        {
            return Result.Failure(BankAccountErrors.DoublicatedBankAccountNumber);
        }

        bool validIban = _unitOfWorkRepository.BankAccounts
            .IsExist(tm => tm.Iban == request.Iban);

        if (validAccountNumber)
        {
            return Result.Failure(BankAccountErrors.DoublicatedIban);
        }

        return Result.Success();
    }

    private Result IsCertificateExist(int id)
    {
        bool isExist = _unitOfWorkRepository.BankAccounts
            .IsExist(cr => cr.Id == id);

        if (!isExist)
        {
            return Result.Failure(BankAccountErrors.NotFoundCertificate);

        }

        return Result.Success();
    }
}
