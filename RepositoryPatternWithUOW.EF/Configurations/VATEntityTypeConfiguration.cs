namespace Otlob.EF.Configurations;

public class VATEntityTypeConfiguration : IEntityTypeConfiguration<VAT>
{
    public void Configure(EntityTypeBuilder<VAT> builder)
    {
        builder
            .HasQueryFilter(ba => EFCore.Property<bool>(ba, "IsDeleted") == false);

        builder
            .Property(v => v.VatNumber)
            .HasMaxLength(20);

        builder
            .HasIndex(tm => new { tm.RestaurantId, tm.VatNumber })
            .IsUnique();
    }
}
