using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }
        public DbSet<UserComplaint> UserComplaints { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrderedMeals> OrderedMeals { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<MealsInOrder> MealsInOrder { get; set; }
        public DbSet<MealPriceHistory> MealsPriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Rate)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.DeliveryDuration)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.DeliveryFee)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Meal>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Meal>()
                .HasIndex(m => m.RestaurantId);

            modelBuilder.Entity<MealPriceHistory>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<MealPriceHistory>()
                .HasIndex(m => m.MealId);

            modelBuilder.Entity<MealPriceHistory>()
                .HasIndex(m => m.StartDate);

            modelBuilder.Entity<Cart>()
                .HasIndex(m => m.ApplicationUserId);

            modelBuilder.Entity<Cart>()
                .HasIndex(m => m.RestaurantId);

            modelBuilder.Entity<OrderedMeals>()
                .Property(r => r.PricePerMeal)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<OrderedMeals>()
               .HasIndex(m => m.MealId);

            modelBuilder.Entity<MealsInOrder>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<MealsInOrder>()
               .HasIndex(m => m.MealId);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.AddressId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.RestaurantId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderPrice);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);




        }
    }
}
