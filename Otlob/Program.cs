using Serilog;

namespace Otlob;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
       
        builder.Services.AddDenpendencies(builder.Configuration);

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseExceptionHandler("/customer/Home/Error");

        app.UseHttpsRedirection();
        
        app.UseStaticFiles();

        app.UseSession();
        
        app.UseCors();

        app.UseRouting();
        
        app.MapRazorPages();

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.UseHangfireDashboard("/SuperAdmin/Hangfire/Dashboard", new DashboardOptions()
        {
            Authorization = 
            [
                new HangfireCustomBasicAuthenticationFilter 
                {
                    User = builder.Configuration["HangfireSettings:UserName"],
                    Pass = builder.Configuration["HangfireSettings:Password"]
                }                    
            ],
            DashboardTitle = "Otlob Hangfire Dashboard",
            DarkModeEnabled = true
        });

        // Analytics Recurring Jobs
        var scopeFactore = app.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactore.CreateScope();
        
        // Admin Analytics
        var adminDailyAnalyticsService = scope.ServiceProvider.GetRequiredService<IAdminDailyAnalyticsService>();
        var adminMonthlyAnalyticsService = scope.ServiceProvider.GetRequiredService<IAdminMonthlyAnalyticsService>();

        RecurringJob.AddOrUpdate("AddAdminDailyAnalyticsJob", () => adminDailyAnalyticsService.Add(), Cron.Daily);
        RecurringJob.AddOrUpdate("AddAdminMonthlyAnalyticsJob", () => adminMonthlyAnalyticsService.Add(), Cron.Monthly);

        // Restaurant Analytics
        var restaurantMonthlyAnalyticsService = scope.ServiceProvider.GetRequiredService<IRestaurantMonthlyAnalyticsService>();
        var restaurantDailyAnalyticsService = scope.ServiceProvider.GetRequiredService<IRestaurantDailyAnalyticsService>();

        RecurringJob.AddOrUpdate("AddRestaurantDailyAnalyticsJob", () => restaurantDailyAnalyticsService.AddForAllActiveRestaurants(), Cron.Daily);
        RecurringJob.AddOrUpdate("AddRestaurantMonthlyAnalyticsJob", () => restaurantMonthlyAnalyticsService.AddForAllActiveRestaurants(), Cron.Monthly);


        app.UseRateLimiter();

        app.MapHub<OrdersHub>("/orderHub");

        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Customer}/{controller=Home}/{action=Home}"
        );

        app.Run();
    }
}
