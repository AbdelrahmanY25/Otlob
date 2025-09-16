namespace Otlob.EF.Configurations;

public class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder
            .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder
            .HasIndex(c => c.RestaurantId);

        builder
            .HasIndex(c => c.UserId);
    }
}
