namespace Otlob.EF.Configurations
{
    public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasQueryFilter(add => EFCore.Property<bool>(add, "IsDeleted") == false);

            builder.Property(u => u.CustomerAddres)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
