namespace Otlob.EF.Configurations;

public class AdminDailyAnalyticsEntityTypeConfiguration : IEntityTypeConfiguration<AdminDailyAnalytic>
{
    public void Configure(EntityTypeBuilder<AdminDailyAnalytic> builder)
    {
        builder.HasIndex(ada => ada.Date).IsUnique();
        
        builder.Property(ada => ada.TotalOrdersSales)
               .HasColumnType("decimal(9,2)");

        builder.Property(ada => ada.TotalOrdersRevenue)
                .HasColumnType("decimal(9,2)")
               .HasComputedColumnSql("([TotalOrdersSales] * 0.10)", true);
        
        builder.Property(ada => ada.AverageOrderPrice)
               .HasColumnType("decimal(9,2)")
               .HasComputedColumnSql("CASE WHEN [CompletedOrdersCount] = 0 THEN 0 ELSE ([TotalOrdersSales] / [CompletedOrdersCount]) END", true);
    }
}
