namespace Otlob.EF.Configurations;

public class ApplicationRoleEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder
            .HasData(
               [
                 new ApplicationRole
                 {
                    Id = DefaultRoles.SuperAdminRoleId,
                    Name = DefaultRoles.SuperAdmin,
                    NormalizedName = DefaultRoles.SuperAdmin.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.SuperAdminRoleConcurrencyStamp
                 },
                 new ApplicationRole
                 {
                     Id = DefaultRoles.RestaurantAdminRoleId,
                     Name = DefaultRoles.RestaurantAdmin,
                     NormalizedName = DefaultRoles.RestaurantAdmin.ToUpper(),
                     ConcurrencyStamp = DefaultRoles.RestaurantAdminRoleConcurrencyStamp
                 },
                 new ApplicationRole
                 {
                     Id = DefaultRoles.CustomerRoleId,
                     Name = DefaultRoles.Customer,
                     NormalizedName = DefaultRoles.Customer.ToUpper(),
                     ConcurrencyStamp = DefaultRoles.CustomerRoleConcurrencyStamp,
                     IsDefault = true
                 }
               ]
            );
    }
}
