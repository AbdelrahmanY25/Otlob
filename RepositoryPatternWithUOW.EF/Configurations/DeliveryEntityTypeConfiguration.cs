using EFCore = Microsoft.EntityFrameworkCore.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otlob.Core.Models;

namespace Otlob.EF.Configurations
{
    public class DeliveryEntityTypeConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasQueryFilter(d => EFCore.Property<bool>(d, "IsDeleted") == false);
        }
    }
}
