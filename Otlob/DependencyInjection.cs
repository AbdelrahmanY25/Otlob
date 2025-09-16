using Stripe;

namespace Otlob
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDenpendencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();

            services.AddControllersWithViews();

            services.AddRazorPages();

            services.AddCachingSession();

            services.AddHangfireConfiguration(configuration);

            services.AddDbContext(configuration);
            
            services.AddIdentityConfigurations();

            services.AddAutoMapperConfiguration();

            services.AddAutoValidationConfig();

            services.AddSenMailsConfiguration(configuration);

            services.AddImageConfiguration(configuration);

            services.AddStripeConfigurations(configuration);

            services.AddUnitOfWork();

            services.AddServices();

            return services;
        }

        private static IServiceCollection AddCachingSession(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(45);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            return services;
        }

        private static IServiceCollection AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(options =>
            {
                options.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
                options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
                options.UseSimpleAssemblyNameTypeSerializer();
                options.UseRecommendedSerializerSettings();
            });

            services.AddHangfireServer();

            return services;
        }
        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite()
                );
            });

            return services;
        }
        private static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 4;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            return services;
        }

        private static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            
            return services;
        }

        private static IServiceCollection AddAutoValidationConfig(this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(typeof(ApplicationUserlVMValidator).Assembly);

            return services;
        }

        private static IServiceCollection AddSenMailsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection(MailSettings.SectionName));
            
            return services;
        }

        private static IServiceCollection AddImageConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ImageSettings>(configuration.GetSection(ImageSettings.SectionName));
            
            return services;
        }

        private static IServiceCollection AddStripeConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            StripeSettings stripeSettings = configuration.GetSection(StripeSettings.SectionName).Get<StripeSettings>()!;

            services.AddOptions<StripeSettings>()
                .BindConfiguration(StripeSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            StripeConfiguration.ApiKey = stripeSettings.SecretKey;

            return services;
        }

        private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IExportReeportsAsExcelService, ExportReeportsAsExcelService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IMealPriceHistoryService, MealPriceHistoryService>();
            services.AddScoped<IMealService, MealService>();
            services.AddScoped<IOrdersAnalysisService, OrdersAnalysisService>();
            services.AddScoped<IOrderDetailsService, OrderDetailsService>();
            services.AddScoped<IOrderedMealsService, OrderedMealsService>();
            services.AddScoped<IOrderService, Services.OrderService>();
            services.AddScoped<IPaginationService, PaginationService>();
            services.AddScoped<IRestaurantProfileService, RestaurantProfileService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IRestauranStatusService, RestauranStatusService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<ISendEmailsToUsersService, SendEmailsToUsersService>();
            services.AddScoped<ITempOrderService, TempOrderService>();
            services.AddScoped<IUsersAnalysisService, UsersAnalysisService>();
            services.AddScoped<IAddPartnerService, AddPartnerService>();
            services.AddScoped<ICategoriesService, CategoriesService>();

            return services;
        }
    }
}
