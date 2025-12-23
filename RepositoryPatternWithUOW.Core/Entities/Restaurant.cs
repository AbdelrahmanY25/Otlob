namespace Otlob.Core.Entities;

public sealed class Restaurant
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumberOfBranches { get; set; } = 1;
    public decimal DeliveryFee { get; set; }
    public decimal DeliveryDuration { get; set; }
    public AcctiveStatus AcctiveStatus { get; set; } = AcctiveStatus.UnAccepted;
    public ProgressStatus ProgressStatus { get; set; } = ProgressStatus.Pending;
    public BusinessType BusinessType { get; set; } = BusinessType.Restaurant;
    public AdministratorRole AdministratorRole { get; set; } = AdministratorRole.Owner;
    public string? Image { get; set; }
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }

    public bool IsOpen => TimeOnly.FromDateTime(DateTime.Now) >= OpeningTime && TimeOnly.FromDateTime(DateTime.Now) < ClosingTime;
    public bool IsClosed => !IsOpen;

    public ApplicationUser Owner { get; set; } = default!;

    public ICollection<RestaurantBranch> RestaurantBranches { get; set; } = [];

    public ICollection<RestaurantCategory> RestaurantCategories { get; set; } = [];

    public ICollection<MenuCategory> MenueCategories { get; set; } = [];
    
    public ICollection<MealAddOn> MealAddOns { get; set; } = [];

    public ICollection<Order> Orders { get; set; } = [];

    public CommercialRegistration CommercialRegistration { get; set; } = default!;

    public TradeMark TradeMark { get; set; } = default!;

    public NationalId NationalId { get; set; } = default!;

    public BankAccount BankAccount { get; set; } = default!;    

    public VAT VatCertificate { get; set; } = default!;    
}

public enum AcctiveStatus
{
    Acctive,
    Block,
    Warning,
    Pending,
    UnAccepted,
    Rejected
}

public enum ProgressStatus
{
    Pending = 1,
    RestaurantProfileCompleted,
    CommercialRegistrationCompleted,
    TradeMarkCompleted,
    VatCertificateCompleted,
    BankAccountCompleted,
    NationalIdCompleted,
    Completed
}

public enum BusinessType
{
    Restaurant,
    CloudKitchen,
    StreetFood
}

public enum AdministratorRole
{
    Owner,
    Partner,
    Manger
}
