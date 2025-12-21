namespace Otlob.EF.Configurations;

public class MealEntityTypeConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder
            .HasOne(m => m.Category)
            .WithMany(c => c.Meals)
            .HasForeignKey(m => m.CategoryId);

        builder
            .HasQueryFilter(m => EFCore.Property<bool>(m, "IsDeleted") == false);

        builder
            .Property(m => m.Name)
            .HasMaxLength(25);

        builder
            .Property(m => m.Description);

        builder
            .Property(m => m.NumberOfServings)
            .HasMaxLength(50);

        builder
            .Property(m => m.Price)
            .HasColumnType("decimal(8,2)");

        builder
            .HasIndex(m => new { m.Id, m.RestaurantId, m.CategoryId })
            .IsUnique();       
    }
}
