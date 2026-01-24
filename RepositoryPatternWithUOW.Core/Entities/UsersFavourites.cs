namespace Otlob.Core.Entities;

public class UsersFavourites
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RestaurantId { get; set; }

    public ApplicationUser User { get; set; } = default!;
    public Restaurant Restaurant { get; set; } = default!;
}
