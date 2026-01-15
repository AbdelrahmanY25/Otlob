namespace Otlob.EF.Configurations;

public class PromoCodeEntityTypeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder
            .HasQueryFilter(b => EFCore.Property<bool>(b, "IsDeleted") == false);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.DiscountType)
            .HasConversion(
                d => d.ToString(),
                d => Enum.Parse<DiscountType>(d)
            );

        builder.Property(p => p.DiscountValue)
            .HasColumnType("decimal(8,2)")
            .IsRequired();

        builder.Property(p => p.MinOrderAmount)
            .HasColumnType("decimal(8,2)");

        builder.Property(p => p.MaxDiscountAmount)
            .HasColumnType("decimal(8,2)");

        builder.HasOne(p => p.Restaurant)
            .WithMany()
            .HasForeignKey(p => p.RestaurantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.RestaurantId);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => new { p.ValidFrom, p.ValidTo });

        // Seed WELCOME25 promo code for new users
        builder.HasData(new PromoCode
        {
            Id = 1,
            Code = "WELCOME25",
            Description = "Welcome discount! Get 25% off on your first order.",
            DiscountType = DiscountType.Percentage,
            DiscountValue = 25,
            MinOrderAmount = null,
            MaxDiscountAmount = null,
            ValidFrom = new DateTime(2024, 1, 1),
            ValidTo = new DateTime(2099, 12, 31),
            MaxTotalUsage = null,
            MaxUsagePerUser = 1,
            IsActive = true,
            IsFirstOrderOnly = true,
            RestaurantId = null,
            CreatedByUserId = DefaultUsers.AdminId,
            CreatedAt = new DateTime(2024, 1, 1)
        });
    }
}
