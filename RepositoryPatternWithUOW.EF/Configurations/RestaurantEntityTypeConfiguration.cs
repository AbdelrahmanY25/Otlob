using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otlob.Core.Models;
using EFCore = Microsoft.EntityFrameworkCore.EF;


namespace Otlob.EF.Configurations
{
    public class RestaurantEntityTypeConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.HasQueryFilter(r => EFCore.Property<bool>(r, "IsDeleted") == false);

            builder.Property(r => r.Rate)
               .HasColumnType("decimal(7,2)");

            builder.Property(r => r.DeliveryDuration)
                .HasColumnType("decimal(5,2)");

            builder.Property(r => r.DeliveryFee)
                .HasColumnType("decimal(5,2)");

            builder.Property(r => r.AcctiveStatus)
                .HasConversion(
                    r => r.ToString(),
                    r => (AcctiveStatus)Enum.Parse(typeof(AcctiveStatus), r)
                )
                .HasColumnType("VARCHAR")
                .HasMaxLength(10);

            builder.Property(r => r.Category)
                .HasConversion(
                    r => r.ToString(),
                    r => (RestaurantCategory)Enum.Parse(typeof(RestaurantCategory), r)
                )
                .HasColumnType("VARCHAR")
                .HasMaxLength(20);
        }
    }
}
