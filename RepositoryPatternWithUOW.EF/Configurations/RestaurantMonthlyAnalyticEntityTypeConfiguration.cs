namespace Otlob.EF.Configurations;

public class RestaurantMonthlyAnalyticEntityTypeConfiguration : IEntityTypeConfiguration<RestaurantMonthlyAnalytic>
{
    public void Configure(EntityTypeBuilder<RestaurantMonthlyAnalytic> builder)
    {
        builder
            .HasQueryFilter(r => EFCore.Property<bool>(r, "IsDeleted") == false);

        builder.HasIndex(rma => new { rma.RestaurantId, rma.Year, rma.Month }).IsUnique();

        builder.HasOne(rma => rma.Restaurant)
               .WithMany()
               .HasForeignKey(rma => rma.RestaurantId);

        builder.Property(rma => rma.TotalOrdersSales)
               .HasColumnType("decimal(12,2)");

        builder.Property(rma => rma.TotalOrdersRevenue)
               .HasColumnType("decimal(12,2)")
               .HasComputedColumnSql("([TotalOrdersSales] * 0.90)", true);
    }
}
