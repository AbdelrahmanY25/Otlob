namespace Otlob.Core.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7().ToString();
    }
    public bool IsDefault { get; set; }
}
