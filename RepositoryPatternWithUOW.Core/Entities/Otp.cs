namespace Otlob.Core.Entities;

public class Otp
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    
    public DateTime ExpiredAt { get; }

    public ApplicationUser User { get; set; } = default!;
}
