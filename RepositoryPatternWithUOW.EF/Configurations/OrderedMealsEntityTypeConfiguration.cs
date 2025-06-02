namespace Otlob.EF.Configurations
{
    public class OrderedMealsEntityTypeConfiguration : IEntityTypeConfiguration<OrderedMeals>
    {
        public void Configure(EntityTypeBuilder<OrderedMeals> builder)
        {
            builder.HasQueryFilter(om => EFCore.Property<bool>(om, "IsDeleted") == false);

            builder.Property(om => om.PricePerMeal)
                .HasColumnType("decimal(8,2)");

            builder.HasIndex(om => om.MealId);
        }
    }
}
