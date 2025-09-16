namespace Otlob.Core.Entities;

public class Cart : AuditEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RestaurantId { get; set; }

    [ValidateNever]
    public ApplicationUser User { get; set; } = null!;

    [ValidateNever]
    public ICollection<CartDetails> CartDetails { get; set; } = null!;

    [ValidateNever]
    public Restaurant Restaurant { get; set; } = null!;
}
