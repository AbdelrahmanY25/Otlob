namespace Otlob.EF.Configurations
{
    public class OrderDetailsEntityTypeConfiguration : IEntityTypeConfiguration<OrderDetails>
    {
        public void Configure(EntityTypeBuilder<OrderDetails> builder)
        {
            builder.HasQueryFilter(od => EFCore.Property<bool>(od, "IsDeleted") == false);

            builder.Property(od => od.MealPrice)
                .HasColumnType("decimal(8,2)");

            builder.Property(od => od.TotalPrice)
                .HasColumnType("decimal(8,2)");

            builder.HasIndex(od => od.MealId);

            builder.Property(od => od.TotalPrice)
                .HasComputedColumnSql("[MealPrice] * [MealQuantity]");
        }
    }
}
