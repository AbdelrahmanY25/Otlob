namespace Otlob.EF.Configurations;

public class MealsAnalyticEntityTypeConfiguration : IEntityTypeConfiguration<MealsAnalytic>
{
    public void Configure(EntityTypeBuilder<MealsAnalytic> builder)
    {
        builder
            .HasQueryFilter(m => EFCore.Property<bool>(m, "IsDeleted") == false);

        builder
            .HasOne(ma => ma.Meal)
            .WithMany(m => m.MealsAnalytics)
            .HasForeignKey(ma => ma.MealId);

        builder
            .Property(ma => ma.Sales)
            .HasColumnType("decimal(10,2)");

        builder
            .HasIndex(ma => new { ma.RestaurantId, ma.MealId })
            .IsUnique();
    }
}
