
namespace Otlob.EF.Configurations;

public class MealOptionItemEntityTypeConfiGuration : IEntityTypeConfiguration<MealOptionItem>
{
    public void Configure(EntityTypeBuilder<MealOptionItem> builder)
    {
        builder
            .HasQueryFilter(e => EFCore.Property<bool>(e, "IsDeleted") == false);
        
            builder
            .Property(e => e.Name)
            .HasMaxLength(150);

        builder
            .Property(e => e.Price)
            .HasColumnType("decimal(8,2)");
    }
}
