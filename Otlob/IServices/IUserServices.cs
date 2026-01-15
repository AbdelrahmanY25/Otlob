namespace Otlob.IServices;

public interface IUserServices
{
    Task<IOrderedEnumerable<UserMainResponse>?> GetAllCustomers();    
    Task<Result> ToggleUserBlockStatusAsync(string userId);
    Task<Result> ToggleConfirmEmailAsync(string userId);
}
