namespace Otlob.EF.Configurations;

public class MealAddOnEntityTypeConfiGuration : IEntityTypeConfiguration<MealAddOn>
{
    public void Configure(EntityTypeBuilder<MealAddOn> builder)
    {
        builder
            .HasQueryFilter(e => EFCore.Property<bool>(e, "IsDeleted") == false);

        builder
            .Property(e => e.Name)
            .HasMaxLength(100);

        builder
            .Property(e => e.Price)
            .HasColumnType("decimal(18,2)");
    }
}
