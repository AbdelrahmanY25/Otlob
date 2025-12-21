namespace Otlob.EF.Configurations;

public class MealOptionGroupEntityTypeConfiGuration : IEntityTypeConfiguration<MealOptionGroup>
{
    public void Configure(EntityTypeBuilder<MealOptionGroup> builder)
    {
        builder
            .HasQueryFilter(e => EFCore.Property<bool>(e, "IsDeleted") == false);

        builder
            .Property(e => e.Name)
            .HasMaxLength(50);
    }
}
