namespace Otlob.EF.Configurations;

public class CommercialRegistrationEntityTypeConfiguration : IEntityTypeConfiguration<CommercialRegistration>
{
    public void Configure(EntityTypeBuilder<CommercialRegistration> builder)
    {
        builder
            .HasQueryFilter(ba => EFCore.Property<bool>(ba, "IsDeleted") == false);

        builder
            .Property(cr => cr.RegistrationNumber)
            .HasMaxLength(9);
        
        builder
            .HasIndex(cr => new { cr.RestaurantId, cr.RegistrationNumber })
            .IsUnique();
    }
}
