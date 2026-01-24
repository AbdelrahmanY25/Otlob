namespace Otlob.EF.Configurations;

public class AdvertisementPaymentEntityTypeConfiguration : IEntityTypeConfiguration<AdvertisementPayment>
{
    public void Configure(EntityTypeBuilder<AdvertisementPayment> builder)
    {
        builder
           .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("EGP");

        builder.Property(p => p.StripeSessionId)
            .HasMaxLength(200);

        builder.Property(p => p.StripePaymentIntentId)
            .HasMaxLength(100);

        builder.Property(p => p.StripeChargeId)
            .HasMaxLength(100);

        builder.Property(p => p.PaymentStatus)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<AdvertisementPaymentStatus>(s)
            );

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(p => p.CardLast4)
            .HasMaxLength(4);

        builder.Property(p => p.CardBrand)
            .HasMaxLength(20);

        builder.Property(p => p.RefundReason)
            .HasMaxLength(500);

        builder.Property(p => p.StripeRefundId)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(p => p.Advertisement)
            .WithOne(a => a.Payment)
            .HasForeignKey<AdvertisementPayment>(p => p.AdvertisementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Restaurant)
            .WithMany(r => r.AdvertisementPayments)
            .HasForeignKey(p => p.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.StripeSessionId);
        builder.HasIndex(p => p.RestaurantId);
        builder.HasIndex(p => p.PaymentStatus);
        builder.HasIndex(p => p.PaidAt);
    }
}
