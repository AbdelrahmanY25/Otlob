namespace Otlob.IServices
{
    public interface IAuthService
    {
        Task<Result<string>> RegisterAsync(ApplicationUserVM userVM, List<string> roles);
        Task<Result<string>> LoginAsync(LoginVM loginVM);
        Task<Result> ResetPasswordAsync(ResetPasswordVM model);
        Task<Result<string>> ChangePasswordAsync(ChangePasswordVM changePsswordVM);
        Task LogOutAsync();
    }
}
