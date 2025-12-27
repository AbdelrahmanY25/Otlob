namespace Otlob.EF.Configurations;

internal class ManyMealManyAddOnEntityTypeConfiguration : IEntityTypeConfiguration<ManyMealManyAddOn>
{
    public void Configure(EntityTypeBuilder<ManyMealManyAddOn> builder)
    {
        builder
            .HasQueryFilter(e => EFCore.Property<bool>(e, "IsDeleted") == false);

        builder
            .HasOne(ma => ma.Meal)
            .WithMany(m => m.MealAddOns)
            .HasForeignKey(ma => ma.MealId);

        builder
            .HasOne(ma => ma.AddOn)
            .WithMany(a => a.MealAddOns)
            .HasForeignKey(ma => ma.AddOnId);

        builder
            .HasKey(ma => new { ma.MealId, ma.AddOnId });

        builder
            .HasIndex(ma => new { ma.MealId, ma.AddOnId })
            .IsUnique();
    }
}
