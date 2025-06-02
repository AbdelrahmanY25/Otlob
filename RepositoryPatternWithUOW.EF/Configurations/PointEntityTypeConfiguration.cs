namespace Otlob.EF.Configurations
{
    public class PointEntityTypeConfiguration : IEntityTypeConfiguration<Point>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Point> builder)
        {
            builder.HasQueryFilter(p => EFCore.Property<bool>(p, "IsDeleted") == false);
        }
    }
}
