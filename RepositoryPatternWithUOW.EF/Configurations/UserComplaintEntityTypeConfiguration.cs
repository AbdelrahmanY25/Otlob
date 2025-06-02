namespace Otlob.EF.Configurations
{
    public class UserComplaintEntityTypeConfiguration : IEntityTypeConfiguration<UserComplaint>
    {
        public void Configure(EntityTypeBuilder<UserComplaint> builder)
        {
            builder.HasQueryFilter(uc => EFCore.Property<bool>(uc, "IsDeleted") == false);
        }
    }
}
