namespace Otlob.EF.Configurations;

public class MenuCategoryEntityTypeConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder
           .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder
            .ToTable("MealCategories");

        builder
            .HasOne(c => c.Restaurant)
            .WithMany(r => r.MenueCategories)
            .HasForeignKey(c => c.RestaurantId);

        builder
            .Property(c => c.Name)
            .HasMaxLength(50);
    }
}
