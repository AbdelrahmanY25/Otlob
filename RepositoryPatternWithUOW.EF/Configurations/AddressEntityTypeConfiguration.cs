namespace Otlob.EF.Configurations;

public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder
            .HasQueryFilter(add => EFCore.Property<bool>(add, "IsDeleted") == false);

        builder
            .Property(a => a.CustomerAddress)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(a => a.StreetName)
            .HasMaxLength(30);

        builder
            .Property(a => a.FloorNumber)
            .HasMaxLength(100);

        builder
            .Property(a => a.CompanyName)
            .HasMaxLength(30);

        builder
            .Property(a => a.HouseNumberOrName)
            .HasMaxLength(20);

        builder
            .Property(a => a.Location)
            .HasColumnType("geography");

        builder
            .Property(a => a.PlaceType)
            .HasMaxLength(20)
            .HasConversion(
                a => a.ToString(),
                a => Enum.Parse<PlaceType>(a)
            );

        builder
            .HasIndex(a => new { a.UserId, a.CustomerAddress, a.StreetName })
            .IsUnique();
    }
}
