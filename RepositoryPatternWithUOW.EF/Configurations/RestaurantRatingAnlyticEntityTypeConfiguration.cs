namespace Otlob.EF.Configurations;

public class RestaurantRatingAnlyticEntityTypeConfiguration : IEntityTypeConfiguration<RestaurantRatingAnlytic>
{
    public void Configure(EntityTypeBuilder<RestaurantRatingAnlytic> builder)
    {
        builder
            .HasQueryFilter(r => EFCore.Property<bool>(r, "IsDeleted") == false);

        builder
            .HasIndex(r => new { r.Id, r.RestaurantId })
            .IsUnique();

        builder
            .Property(r => r.Score)
            .HasColumnType("decimal(12.2)");

        builder
            .Property(r => r.AverageRate)
            .HasColumnType("decimal(12.2)")
            .HasComputedColumnSql("CASE WHEN [RatesCount] = 0 THEN 0 ELSE CAST([Score] / [RatesCount] AS DECIMAL(12,2)) END", stored: true);
    }
}
