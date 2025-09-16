namespace Otlob.Core.Contracts.ViewModel;

public record LoginVM
{
    [ValidEmail]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
