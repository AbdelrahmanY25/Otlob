namespace Otlob.EF.Configurations;

public class NationalIdEntityTypeConfiguration : IEntityTypeConfiguration<NationalId>
{
    public void Configure(EntityTypeBuilder<NationalId> builder)
    {
        builder
            .HasQueryFilter(ba => EFCore.Property<bool>(ba, "IsDeleted") == false);

        builder
            .Property(nid => nid.NationalIdNumber)
            .HasMaxLength(14);
        
        builder
            .Property(nid => nid.FullName)
            .HasMaxLength(100);

        builder
            .Property(nid => nid.Status)
            .HasConversion(
                nid => nid.ToString(),
                nid => Enum.Parse<DocumentStatus>(nid)
            );

        builder
            .HasIndex(nid => new { nid.RestaurantId, nid.NationalIdNumber })
            .IsUnique();
    }
}
