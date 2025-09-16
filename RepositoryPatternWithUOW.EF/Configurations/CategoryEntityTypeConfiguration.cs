namespace Otlob.EF.Configurations;

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasQueryFilter(c => EFCore.Property<bool>(c, "IsDeleted") == false);

        builder
            .Property(c => c.Name)
            .HasMaxLength(20);

        builder.HasData(LoadCategories());
    }

    private static List<Category> LoadCategories()
    {
        List<Category> categories = [];

        categories.Add(new() { Id = 1, Name = "Burger" });
        categories.Add(new() { Id = 2, Name = "Shawarma" });
        categories.Add(new() { Id = 3, Name = "Pizza" });
        categories.Add(new() { Id = 4, Name = "FriedChicken" });
        categories.Add(new() { Id = 5, Name = "EgyptionFood" });
        categories.Add(new() { Id = 6, Name = "IndianFood" });
        categories.Add(new() { Id = 7, Name = "ChineseFood" });
        categories.Add(new() { Id = 8, Name = "JapaneseFood" });
        categories.Add(new() { Id = 9, Name = "ItalianFood" });
        categories.Add(new() { Id = 10, Name = "Sandwiches" });
        categories.Add(new() { Id = 11, Name = "HealthyFood" });
        categories.Add(new() { Id = 12, Name = "SeaFood" });
        categories.Add(new() { Id = 13, Name = "Drinks" });
        categories.Add(new() { Id = 14, Name = "IceCream" });
        categories.Add(new() { Id = 15, Name = "Dessert" });
        categories.Add(new() { Id = 16, Name = "Bakery" });
        categories.Add(new() { Id = 17, Name = "Coffee" });
        categories.Add(new() { Id = 18, Name = "Other" });

        return categories;
    }
}
