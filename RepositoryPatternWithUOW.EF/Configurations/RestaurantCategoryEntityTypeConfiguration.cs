namespace Otlob.EF.Configurations;

public class RestaurantCategoryEntityTypeConfiguration : IEntityTypeConfiguration<RestaurantCategory>
{
    public void Configure(EntityTypeBuilder<RestaurantCategory> builder)
    {
        builder
            .HasOne(rc => rc.Restaurant)
            .WithMany(r => r.RestaurantCategories)
            .HasForeignKey(rc => rc.RestaurantId);

        builder
            .HasOne(rc => rc.Category)
            .WithMany(c => c.RestaurantCategory)
            .HasForeignKey(rc => rc.CategoryId);

        builder
            .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder
            .HasKey(rc => new { rc.CategoryId, rc.RestaurantId });

        builder
            .HasIndex(rc => new { rc.CategoryId, rc.RestaurantId })
            .IsUnique();
    }
}
