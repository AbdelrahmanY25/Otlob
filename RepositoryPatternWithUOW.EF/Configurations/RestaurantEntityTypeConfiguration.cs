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
            .Property(u => u.Image)
            .HasMaxLength(100);
        
        builder
            .Property(r => r.NumberOfBranches)
            .HasMaxLength(5000);

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
            .Property(r => r.AdministratorRole)
            .HasConversion(
                r => r.ToString(),
                r => Enum.Parse<AdministratorRole>(r)
            )
            .HasColumnType("VARCHAR")
            .HasMaxLength(10);

        builder
            .Property(r => r.ProgressStatus)
            .HasConversion(
                r => r.ToString(),
                r => Enum.Parse<ProgressStatus>(r)
            )
            .HasColumnType("VARCHAR")
            .HasMaxLength(50);

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
