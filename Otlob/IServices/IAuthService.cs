namespace Otlob.IServices
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequest request, List<string> roles);
        
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

        Task<Result> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);

        Task<Result<string>> LoginAsync(LoginRequest loginVM);
        
        Task<Result> ForgetPasswordAsync(ForgetPasswordRequest request);
        
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
        
        Task<Result> ChangePasswordAsync(ChangePasswordRequest changePsswordVM);
        
        Task LogOutAsync();
    }
}
