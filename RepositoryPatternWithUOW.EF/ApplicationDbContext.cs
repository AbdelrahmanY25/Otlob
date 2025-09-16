namespace Otlob.EF;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Address> Addresses { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<CartDetails> CartDetails { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CommercialRegistration> CommercialRegistrations { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<MealPriceHistory> MealsPriceHistories { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<NationalId> NationalIds { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<TempOrder> TempOrders { get; set; }
    public DbSet<TradeMark> TradeMarks { get; set; }
    public DbSet<VAT> Vats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var fKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => !fk.IsOwnership);

        foreach ( var fk in fKs )
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {               
            modelBuilder.Entity(entity.ClrType)
                .Property<bool>("IsDeleted")
                .IsRequired()
                .HasDefaultValue(false);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries<AuditEntity>();

        foreach (var entry in entries)
        {
            var currentUserId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedById).CurrentValue = currentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }
}
