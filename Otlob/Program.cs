using Microsoft.EntityFrameworkCore;
using Otlob.Core.Models;
using Microsoft.AspNetCore.Identity;
using Otlob.EF;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.EF.UnitOfWorkRepository;
using Otlob.Core.Hubs;
using Stripe;
using Utility;
using Otlob.Core.Services;
using Otlob.Core.IServices;

namespace Otlob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSignalR();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Add Distributed Memory Cache (Required for Session)
            builder.Services.AddDistributedMemoryCache();

            // Configure Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Prevent client-side script access
                options.Cookie.IsEssential = true; // Ensure session is always available
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication()  
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            builder.Services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.AddScoped<IIdEncryptionService, IdEncryptionService>();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable Session Middleware (Before Routing)
            app.UseSession();

            app.UseRouting();
            app.MapRazorPages();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<OrdersHub>("/orderHub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
