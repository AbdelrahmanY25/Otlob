namespace Otlob.EF.Configurations;

public class AdvertisementPlanEntityTypeConfiguration : IEntityTypeConfiguration<AdvertisementPlan>
{
    public void Configure(EntityTypeBuilder<AdvertisementPlan> builder)
    {
        builder
           .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.NameAr)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.DescriptionAr)
            .HasMaxLength(500);

        builder.Property(p => p.PricePerMonth)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.DurationInDays)
            .HasDefaultValue(30);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(p => p.DisplayOrder);

        // Seed the 3 plans
        builder.HasData(
            new AdvertisementPlan
            {
                Id = 1,
                Name = "Basic",
                NameAr = "أساسي",
                Description = "Standard visibility for your restaurant. Your ad will appear in the restaurants list with a sponsored badge.",
                DescriptionAr = "ظهور عادي لمطعمك. سيظهر إعلانك في قائمة المطاعم مع شارة مُموَّل.",
                PricePerMonth = 500,
                DurationInDays = 30,
                DisplayOrder = 3,
                IsActive = true
            },
            new AdvertisementPlan
            {
                Id = 2,
                Name = "Premium",
                NameAr = "مميز",
                Description = "Higher priority display. Your ad will appear at the top of the restaurants list and in the home page carousel.",
                DescriptionAr = "أولوية عرض أعلى. سيظهر إعلانك في أعلى قائمة المطاعم وفي شريط الصفحة الرئيسية.",
                PricePerMonth = 1000,
                DurationInDays = 30,
                DisplayOrder = 2,
                IsActive = true
            },
            new AdvertisementPlan
            {
                Id = 3,
                Name = "Featured",
                NameAr = "متميز",
                Description = "Top position with highlighted border. Your ad will be featured at the very top of all pages with special styling.",
                DescriptionAr = "أعلى موضع مع إطار مميز. سيتم عرض إعلانك في أعلى جميع الصفحات بتنسيق خاص.",
                PricePerMonth = 2000,
                DurationInDays = 30,
                DisplayOrder = 1,
                IsActive = true
            }
        );
    }
}
