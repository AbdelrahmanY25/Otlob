namespace Otlob.EF.Configurations;

public class RestaurantDailyAnalyticsEntityTypeConfiguration : IEntityTypeConfiguration<RestaurantDailyAnalytic>
{
    public void Configure(EntityTypeBuilder<RestaurantDailyAnalytic> builder)
    {
        builder
            .HasQueryFilter(r => EFCore.Property<bool>(r, "IsDeleted") == false);

        builder.HasIndex(rda => new { rda.RestaurantId, rda.Date }).IsUnique();
        
        builder.HasOne(rda => rda.Restaurant)
               .WithMany()
               .HasForeignKey(rda => rda.RestaurantId);

        builder.Property(rda => rda.TotalOrdersSales)
               .HasColumnType("decimal(12,2)");

        builder.Property(rda => rda.TotalOrdersRevenue)
               .HasColumnType("decimal(12,2)")
               .HasComputedColumnSql("([TotalOrdersSales] * 0.90)", true);
        
        builder.Property(rda => rda.AverageOrderPrice)
               .HasColumnType("decimal(12,2)")
               .HasComputedColumnSql("CASE WHEN [CompletedOrdersCount] = 0 THEN 0 ELSE ([TotalOrdersSales] / [CompletedOrdersCount]) END", true);
    }
}
