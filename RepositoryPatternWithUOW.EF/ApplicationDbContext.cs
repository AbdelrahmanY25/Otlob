using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Otlob.Core.Models;

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
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<MealPriceHistory> MealsPriceHistories { get; set; }
        public DbSet<TempOrder> TempOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var item in modelBuilder.Model.GetEntityTypes())
            {               
                modelBuilder.Entity(item.ClrType)
                    .Property<bool>("IsDeleted")
                    .IsRequired()
                    .HasDefaultValue(false);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);     
        }
    }
}
