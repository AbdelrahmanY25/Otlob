namespace Otlob.EF.Configurations
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasQueryFilter(u => EFCore.Property<bool>(u, "IsDeleted") == false);           
        }
    }
}
