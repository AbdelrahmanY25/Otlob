namespace Otlob.Core.Contracts.ViewModel;

public class ForgetPasswordVM
{
    [ValidEmail]
    public string Email { get; set; } = string.Empty;
}
