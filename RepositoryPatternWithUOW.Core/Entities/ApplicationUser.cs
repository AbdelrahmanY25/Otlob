namespace Otlob.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }
 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Image { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Address> UserAddress { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}

public enum Gender
{
    Male,
    Female
}
