namespace Utility.Settings;

public class JwtOptions
{
    public static string SectionName { get; } = "JWT";

    [Required]
    public string Key { get; init; } = string.Empty;

    [Required]
    public string Issure { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }
}
