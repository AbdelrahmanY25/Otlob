namespace Otlob.EF.Configurations;

public class IdentityUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder
            .HasData(
                new IdentityUserRole<string>
                {
                    RoleId = DefaultRoles.SuperAdminRoleId,
                    UserId = DefaultUsers.AdminId,
                }
            );
    }
}
