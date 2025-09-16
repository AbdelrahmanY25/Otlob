namespace Otlob.Core.Contracts.ViewModel;

public class ResetPasswordVM
{
    [ValidEmail]
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;    
}