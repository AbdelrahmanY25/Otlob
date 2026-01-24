namespace Otlob.EF.Configurations;

public class AdvertisementEntityTypeConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder
           .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.TitleAr)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .HasMaxLength(300);

        builder.Property(a => a.DescriptionAr)
            .HasMaxLength(300);

        builder.Property(a => a.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Status)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<AdvertisementStatus>(s)
            );

        builder.Property(a => a.RejectionReason)
            .HasMaxLength(500);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(a => a.Restaurant)
            .WithMany(r => r.Advertisements)
            .HasForeignKey(a => a.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AdvertisementPlan)
            .WithMany(p => p.Advertisements)
            .HasForeignKey(a => a.AdvertisementPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ReviewedByUser)
            .WithMany()
            .HasForeignKey(a => a.ReviewedByUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.RestaurantId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.StartDate);
        builder.HasIndex(a => a.EndDate);
        builder.HasIndex(a => new { a.Status, a.StartDate, a.EndDate });
    }
}
