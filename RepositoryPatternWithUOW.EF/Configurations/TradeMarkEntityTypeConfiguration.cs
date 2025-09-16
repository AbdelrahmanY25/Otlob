namespace Otlob.EF.Configurations;

public class TradeMarkEntityTypeConfiguration : IEntityTypeConfiguration<TradeMark>
{
    public void Configure(EntityTypeBuilder<TradeMark> builder)
    {
        builder
            .HasQueryFilter(ba => EFCore.Property<bool>(ba, "IsDeleted") == false);

        builder
            .Property(tm => tm.TrademarkName)
            .HasMaxLength(30);
        
        builder
            .Property(tm => tm.TrademarkNumber)
            .HasMaxLength(10);

        builder
            .HasIndex(tm => new { tm.RestaurantId, tm.TrademarkNumber })
            .IsUnique();
    }
}
