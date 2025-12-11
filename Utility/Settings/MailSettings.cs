namespace Utility.Consts;

public class MailSettings
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(100, 999)]
    public int Port { get; set; }
}
