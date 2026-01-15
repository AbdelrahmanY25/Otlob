namespace Otlob.EF.Configurations;

public class PromoCodeUsageEntityTypeConfiguration : IEntityTypeConfiguration<PromoCodeUsage>
{
    public void Configure(EntityTypeBuilder<PromoCodeUsage> builder)
    {
        builder
            .HasQueryFilter(b => EFCore.Property<bool>(b, "IsDeleted") == false);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.DiscountApplied)
            .HasColumnType("decimal(8,2)")
            .IsRequired();

        builder.HasOne(u => u.PromoCode)
            .WithMany(p => p.Usages)
            .HasForeignKey(u => u.PromoCodeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Order)
            .WithMany()
            .HasForeignKey(u => u.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.PromoCodeId);
        builder.HasIndex(u => u.UserId);
        builder.HasIndex(u => new { u.PromoCodeId, u.UserId });
    }
}
