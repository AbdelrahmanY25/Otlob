namespace Otlob.EF.Configurations;

public class MealPriceHistoryEntityTypeConfiguration : IEntityTypeConfiguration<MealPriceHistory>
{
    public void Configure(EntityTypeBuilder<MealPriceHistory> builder)
    {
        builder
            .HasQueryFilter(oph => EFCore.Property<bool>(oph, "IsDeleted") == false);

        builder
            .Property(mph => mph.Price)
            .HasColumnType("decimal(8,2)");

        builder
            .HasIndex(mph => mph.MealId);

        builder
            .HasIndex(mph => mph.StartDate);
    }
}
