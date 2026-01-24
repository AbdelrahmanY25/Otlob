namespace Otlob.EF.Configurations;

public class UsersFavouritesEntityTypeConfiguration : IEntityTypeConfiguration<UsersFavourites>
{
    public void Configure(EntityTypeBuilder<UsersFavourites> builder)
    {
        builder
            .HasQueryFilter(f => EFCore.Property<bool>(f, "IsDeleted") == false);

        builder
            .HasKey(f => f.Id);

        builder
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(f => f.Restaurant)
            .WithMany()
            .HasForeignKey(f => f.RestaurantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(f => new { f.UserId, f.RestaurantId })
            .IsUnique();
    }
}
