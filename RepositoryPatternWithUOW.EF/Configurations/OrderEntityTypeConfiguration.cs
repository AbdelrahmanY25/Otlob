using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otlob.Core.Models;
using EFCore = Microsoft.EntityFrameworkCore.EF;


namespace Otlob.EF.Configurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasQueryFilter(o => EFCore.Property<bool>(o, "IsDeleted") == false);

            builder
               .Property(o => o.TotalOrderPrice)
               .HasColumnType("decimal(8,2)");

            builder.Property(o => o.TotalMealsPrice)
                .HasColumnType("decimal(8,2)");

            builder.Property(o => o.TotalTaxPrice)
                .HasColumnType("decimal(5,2)");

            builder.Property(o => o.TotalOrderPrice)
                .HasComputedColumnSql("[TotalMealsPrice] + [TotalTaxPrice]");

            builder.Property(o => o.Method)
                .HasConversion(
                    o => o.ToString(),
                    o => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), o)
                );

            builder.Property(o => o.Status)
                .HasConversion(
                    o => o.ToString(),
                    o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
                );


            builder
                .HasIndex(o => o.RestaurantId);

            builder
                .HasIndex(o => o.OrderDate);

            builder
                .HasIndex(o => o.TotalOrderPrice);

            builder
                .HasIndex(o => o.Status);
        }
    }
}
