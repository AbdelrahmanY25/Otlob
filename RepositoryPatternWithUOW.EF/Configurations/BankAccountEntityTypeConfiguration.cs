namespace Otlob.EF.Configurations;

public class BankAccountEntityTypeConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder
            .HasQueryFilter(ba => EFCore.Property<bool>(ba, "IsDeleted") == false);

        builder
            .Property(ba => ba.AccountHolderName)
            .HasMaxLength(100);

        builder
            .Property(ba => ba.AccountNumber)
            .HasMaxLength(19);
        
        builder
            .Property(ba => ba.Iban)
            .HasMaxLength(29);

        builder
            .Property(ba => ba.BankName)
            .HasConversion(
                ba => ba.ToString(),
                ba => Enum.Parse<EgyptianBanks>(ba)
            );

        builder
            .Property(ba => ba.AccountType)
            .HasConversion(
                ba => ba.ToString(),
                ba => Enum.Parse<AccountType>(ba)
            );

        builder
            .Property(ba => ba.Status)
            .HasConversion(
                ba => ba.ToString(),
                ba => Enum.Parse<DocumentStatus>(ba)
            );

        builder
            .HasIndex(ba => new { ba.RestaurantId, ba.AccountNumber })
            .IsUnique();

        builder
            .HasIndex(ba => new { ba.RestaurantId, ba.Iban })
            .IsUnique();
    }
}
