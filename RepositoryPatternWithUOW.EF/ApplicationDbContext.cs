using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
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

        public DbSet<ApplicationUserlVM> ApplicationModelVM { get; set; } = default!;
        public DbSet<LoginVM> LoginVM { get; set; } = default!;
        public DbSet<ProfileVM> ProfileVM { get; set; } = default!;
        public DbSet<ChangePasswordVM> ChangePasswordVM { get; set; } = default!;
    }
}
