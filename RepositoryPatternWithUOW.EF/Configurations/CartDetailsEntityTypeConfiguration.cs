namespace Otlob.EF.Configurations;

public class CartDetailsEntityTypeConfiguration : IEntityTypeConfiguration<CartDetails>
{
    public void Configure(EntityTypeBuilder<CartDetails> builder)
    {
        builder
            .HasQueryFilter(om => EFCore.Property<bool>(om, "IsDeleted") == false);

        builder
            .Property(om => om.MealPrice)
            .HasColumnType("decimal(8,2)");

        builder
            .Property(om => om.ItemsPrice)
            .HasColumnType("decimal(8,2)");
        builder
            .Property(om => om.AddOnsPrice)
            .HasColumnType("decimal(8,2)");

        builder
            .Property(om => om.TotalPrice)
            .HasColumnType("decimal(8,2)")
            .HasComputedColumnSql("[Quantity] * ([MealPrice] + [ItemsPrice] + [AddOnsPrice])", true);

        builder
            .HasIndex(om => om.MealId);
    }
}
