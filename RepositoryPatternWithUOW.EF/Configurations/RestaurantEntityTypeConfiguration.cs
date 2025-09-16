namespace Otlob.EF.Configurations;

public class RestaurantEntityTypeConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder
            .HasMany(r => r.Orders)
            .WithOne(o => o.Restaurant)
            .HasForeignKey(o => o.RestaurantId)
            .IsRequired();

       builder
            .HasQueryFilter(r => EFCore.Property<bool>(r, "IsDeleted") == false);

        builder
            .Property(r => r.Name)
            .HasMaxLength(20);

        builder
            .Property(r => r.Email)
            .HasMaxLength(100);

        builder
            .Property(r => r.Phone)
            .HasMaxLength(11);

        builder
            .Property(r => r.NumberOfBranches)
            .HasMaxLength(500);

        builder
            .Property(r => r.Description)
            .HasMaxLength(300);

        builder
            .Property(r => r.DeliveryDuration)
            .HasColumnType("decimal(5,2)");

        builder
            .Property(r => r.DeliveryFee)
            .HasColumnType("decimal(5,2)");

        builder
            .Property(r => r.AcctiveStatus)
            .HasConversion(
                r => r.ToString(),
                r => Enum.Parse<AcctiveStatus>(r)
            )
            .HasColumnType("VARCHAR")
            .HasMaxLength(10);

        builder
            .Property(r => r.BusinessType)
            .HasConversion(
                r => r.ToString(),
                r => Enum.Parse<BusinessType>(r)
            )
            .HasColumnType("VARCHAR")
            .HasMaxLength(10);

        builder
            .Property(r => r.OwnerRole)
            .HasConversion(
                r => r.ToString(),
                r => Enum.Parse<Role>(r)
            )
            .HasColumnType("VARCHAR")
            .HasMaxLength(10);

        builder
            .HasIndex(r => r.Email)
            .IsUnique();

        builder
            .HasIndex(r => r.Name)
            .IsUnique();

        builder
            .HasIndex(r => r.Phone)
            .IsUnique();
    }
}
