namespace Otlob.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Image { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }

  [ValidateNever]
    public ICollection<Address> UserAddress { get; set; } = null!;

    [ValidateNever]
    public ICollection<Order> Orders { get; set; } = null!;
}

public enum Gender
{
    Male,
    Female
}
