namespace Otlob;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
       
        builder.Services.AddDenpendencies(builder.Configuration);                                                         

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseExceptionHandler("/customer/Home/Error");

        app.UseRateLimittingMiddleware();

        app.UseRequestTimeMiddleware();

        app.UseHttpsRedirection();
        
        app.UseStaticFiles();

        app.UseSession();

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

        app.MapHub<OrdersHub>("/orderHub");

        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Otlob}/{controller=Home}/{action=Home}"
        );

        app.Run();
    }
}
