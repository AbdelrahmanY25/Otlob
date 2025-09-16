namespace Otlob.IServices;

public interface IUserServices
{
    IQueryable<ApplicationUser>? GetAllUsers(Expression<Func<ApplicationUser, bool>>? query = null);
    
    int GetCustomersCount();
    
    Task<Result> ToggleUserBlockStatusAsync(string userId);   
    
    Task<Result<ApplicationUser>> GetUserContactInfo(string userId);    
}
