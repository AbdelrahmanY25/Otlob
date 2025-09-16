namespace Otlob.EF.Configurations;

public class CartDetailsEntityTypeConfiguration : IEntityTypeConfiguration<CartDetails>
{
    public void Configure(EntityTypeBuilder<CartDetails> builder)
    {
        builder
            .HasQueryFilter(om => EFCore.Property<bool>(om, "IsDeleted") == false);

        builder
            .Property(om => om.PricePerMeal)
            .HasColumnType("decimal(8,2)");

        builder
            .HasIndex(om => om.MealId);
    }
}
