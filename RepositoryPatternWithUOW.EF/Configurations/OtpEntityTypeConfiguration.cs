namespace Otlob.EF.Configurations;

public class OtpEntityTypeConfiguration : IEntityTypeConfiguration<Otp>
{
    public void Configure(EntityTypeBuilder<Otp> builder)
    {
        builder
            .HasQueryFilter(o => EFCore.Property<bool>(o, "IsDeleted") == false);

        builder
            .Property(o => o.ExpiredAt)
            .HasComputedColumnSql("DATEADD(SECOND, 90, CreatedAt)", stored: true);
    }
}
