namespace Otlob.IServices;

public interface IBankAccountService
{
    Result<BankAccountResponse> GetBankAccount(string id);
    Task<Result> UploadAsync(BankAccountRequest request, UploadFileRequest fileRequest);
    Result ChangBankAccountStatus(string id, DocumentStatus status);
    bool IsRestaurantHasBankAccountCertificate(int restaurantId);
}
