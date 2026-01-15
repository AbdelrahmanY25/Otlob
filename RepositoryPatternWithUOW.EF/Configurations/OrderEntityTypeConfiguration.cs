namespace Otlob.EF.Configurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasOne(o => o.Restaurant)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => o.RestaurantId)
            .IsRequired();

        builder
            .HasQueryFilter(o => EFCore.Property<bool>(o, "IsDeleted") == false);

        builder
            .Property(o => o.SubPrice)
            .HasColumnType("decimal(8,2)");

        builder
            .Property(o => o.ServiceFeePrice)
            .HasColumnType("decimal(5,2)");
        
        builder
            .Property(o => o.DeliveryFee)
            .HasColumnType("decimal(5,2)");

        builder
            .Property(o => o.DiscountAmount)
            .HasColumnType("decimal(8,2)")
            .HasDefaultValue(0);

        builder
            .Property(o => o.TotalPrice)
            .HasColumnType("decimal(8,2)")
            .HasComputedColumnSql("[SubPrice] + [ServiceFeePrice] + [DeliveryFee] - [DiscountAmount]", true);

        builder
            .HasOne(o => o.PromoCode)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PromoCodeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(o => o.Method)
            .HasConversion(
                o => o.ToString(),
                o => Enum.Parse<PaymentMethod>(o)
            );

        builder
            .Property(o => o.Status)
            .HasConversion(
                o => o.ToString(),
                o => Enum.Parse<OrderStatus>(o)
            );

        builder
            .Property(o => o.CustomerCancelReason)
            .HasConversion(
                o => o.ToString(),
                o => Enum.Parse<CustomerCancelReason>(o)
            );

        builder
            .Property(o => o.RestaurantCancelReason)
            .HasConversion(
                o => o.ToString(),
                o => Enum.Parse<RestaurantCancelReason>(o)
            );

        builder
            .HasIndex(o => o.RestaurantId);

        builder
            .HasIndex(o => o.OrderDate);

        builder
            .HasIndex(o => o.Status);
    }
}
