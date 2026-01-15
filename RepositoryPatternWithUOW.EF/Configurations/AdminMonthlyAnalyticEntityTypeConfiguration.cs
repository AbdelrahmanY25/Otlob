namespace Otlob.EF.Configurations;

public class AdminMonthlyAnalyticEntityTypeConfiguration : IEntityTypeConfiguration<AdminMonthlyAnalytic>
{
    public void Configure(EntityTypeBuilder<AdminMonthlyAnalytic> builder)
    {
        builder
            .HasIndex(x => new { x.Year, x.Month })
            .IsUnique();

        builder
            .Property(x => x.TotalOrdersSales)
            .HasColumnType("decimal(10,2)");

        builder
            .Property(x => x.TotalOrdersRevenue)
            .HasColumnType("decimal(10,2)")
            .HasComputedColumnSql("([TotalOrdersSales] * 0.10)", true);
    }
}
