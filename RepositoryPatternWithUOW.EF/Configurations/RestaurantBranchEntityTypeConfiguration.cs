namespace Otlob.EF.Configurations;

public class RestaurantBranchEntityTypeConfiguration : IEntityTypeConfiguration<RestaurantBranch>
{
    public void Configure(EntityTypeBuilder<RestaurantBranch> builder)
    {
        builder
            .HasQueryFilter(b => EFCore.Property<bool>(b, "IsDeleted") == false);

        builder
            .ToTable("RestaurantBranches");

        builder
            .Property(b => b.Name)
            .HasMaxLength(50);

        builder
            .Property(b => b.Address)
            .HasMaxLength(150);

        builder
            .Property(b => b.City)
            .HasMaxLength(30);

        builder
            .Property(b => b.MangerName)
            .HasMaxLength(50);

        builder
            .Property(b => b.MangerPhone)
            .HasMaxLength(11);

        builder
            .Property(b => b.Location)
            .HasColumnType("geography");

        builder
            .HasIndex(b => new { b.RestaurantId, b.Name })
            .IsUnique();
    }
}
