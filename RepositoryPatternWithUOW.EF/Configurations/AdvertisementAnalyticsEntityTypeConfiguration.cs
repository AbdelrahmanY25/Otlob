namespace Otlob.EF.Configurations;

public class AdvertisementAnalyticsEntityTypeConfiguration : IEntityTypeConfiguration<AdvertisementAnalytics>
{
    public void Configure(EntityTypeBuilder<AdvertisementAnalytics> builder)
    {
        builder
           .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Views)
            .HasDefaultValue(0);

        builder.Property(a => a.Clicks)
            .HasDefaultValue(0);

        builder.Property(a => a.LastUpdated)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationship
        builder.HasOne(a => a.Advertisement)
            .WithOne(ad => ad.Analytics)
            .HasForeignKey<AdvertisementAnalytics>(a => a.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(a => a.AdvertisementId).IsUnique();
    }
}
