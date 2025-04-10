using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otlob.Core.Models;
using EFCore = Microsoft.EntityFrameworkCore.EF;


namespace Otlob.EF.Configurations
{
    public class MealEntityTypeConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.HasQueryFilter(m => EFCore.Property<bool>(m, "IsDeleted") == false);

            builder.Property(m => m.Price)
                .HasColumnType("decimal(8,2)");

            builder.HasIndex(m => m.RestaurantId);

            builder.Property(m => m.Category)
                .HasConversion(
                    m => m.ToString(),
                    m => (MealCategory)Enum.Parse(typeof(MealCategory), m)
                )
                .HasColumnType("VARCHAR")
                .HasMaxLength(20);
        }
    }
}
