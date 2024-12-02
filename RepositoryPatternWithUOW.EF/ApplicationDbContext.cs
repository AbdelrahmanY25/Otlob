using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<CustomerConcern> CustomerConcerns { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
