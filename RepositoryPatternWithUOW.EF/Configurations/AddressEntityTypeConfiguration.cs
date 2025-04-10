using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otlob.Core.Models;
using EFCore = Microsoft.EntityFrameworkCore.EF;


namespace Otlob.EF.Configurations
{
    public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasQueryFilter(add => EFCore.Property<bool>(add, "IsDeleted") == false);
        }
    }
}
