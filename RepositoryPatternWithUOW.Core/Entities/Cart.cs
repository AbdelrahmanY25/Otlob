namespace Otlob.Core.Entities;

public class Cart : AuditEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RestaurantId { get; set; }

    
    public ApplicationUser User { get; set; } = default!; 
    public Restaurant Restaurant { get; set; } = default!;
    public ICollection<CartDetails> CartDetails { get; set; } = [];
}
