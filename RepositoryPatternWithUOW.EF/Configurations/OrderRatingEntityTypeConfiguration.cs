namespace Otlob.EF.Configurations;

public class OrderRatingEntityTypeConfiguration : IEntityTypeConfiguration<OrderRating>
{
    public void Configure(EntityTypeBuilder<OrderRating> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Order)
            .WithOne(o => o.Rating)
            .HasForeignKey<OrderRating>(r => r.OrderId)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(500);

        builder.HasIndex(r => r.OrderId)
            .IsUnique();

        builder.HasQueryFilter(r => EFCore.Property<bool>(r, "IsDeleted") == false);
    }
}
