namespace Otlob.EF.Configurations;

public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder
            .HasQueryFilter(ba => EFCore.Property<bool>(ba, "IsDeleted") == false);

        builder
            .Property(c => c.CommissionRate)
            .HasColumnType("decimal(3,2)");
    }
}
