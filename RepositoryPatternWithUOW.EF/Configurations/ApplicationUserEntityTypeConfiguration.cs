namespace Otlob.EF.Configurations;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .HasQueryFilter(u => EFCore.Property<bool>(u, "IsDeleted") == false);
     
        builder
            .Property(u => u.PhoneNumber)
            .HasMaxLength(11);

        builder
            .Property(u => u.FirstName)
            .HasMaxLength(15);

        builder
            .Property(u => u.LastName)
            .HasMaxLength(15);

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);                

        builder
            .Property(u => u.UserName)
            .IsRequired()   
            .HasMaxLength(20);
        
        builder
            .Property(u => u.Image)
            .HasMaxLength(100);

        builder
            .Property(u => u.Gender)
            .HasConversion(
                u => u.ToString(),
                u => (Gender)Enum.Parse(typeof(Gender), u!)
            )
            .HasColumnType("VARCHAR")
            .HasMaxLength(6);

        builder
            .HasIndex(u => u.Id)
            .IsUnique();

        builder
            .HasIndex(u => u.Email)
            .IsUnique();

        builder
            .HasIndex(u => u.UserName)
            .IsUnique();

        builder
            .HasData(
            new ApplicationUser
                {
                    Id = DefaultUsers.AdminId,
                    UserName = DefaultUsers.AdminUserName,
                    NormalizedUserName = DefaultUsers.AdminUserName.ToUpper(),
                    Email = DefaultUsers.AdminEmail,
                    NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                    PasswordHash = DefaultUsers.AdminPassword,
                    EmailConfirmed = true,
                    SecurityStamp = DefaultUsers.AdminSecurityStamp,
                    ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                    FirstName = "Otlob",
                    LastName = "Admin",
                    LockoutEnabled = true,
                }
            );
    }
}
