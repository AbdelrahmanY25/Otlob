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
    public BusinessType BusinessType { get; set; } = BusinessType.Restaurant;
    public Role OwnerRole { get; set; }
    public string? Image { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime ClosingTime { get; set; }
    public bool IsOpen { get; set; }
    public bool IsClosed => !IsOpen; 

    public ApplicationUser Owner { get; set; } = null!;

    public ICollection<RestaurantBranch> RestaurantBranches { get; set; } = null!;

    public ICollection<RestaurantCategory> RestaurantCategories { get; set; } = null!;

    public ICollection<MenuCategory> MenueCategories { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = null!;

    public CommercialRegistration CommercialRegistration { get; set; } = null!;

    public TradeMark TradeMark { get; set; } = null!;

    public NationalId NationalId { get; set; } = null!;

    public BankAccount BankAccount { get; set; } = null!;    

    public VAT VatCertificate { get; set; } = null!;    

    public Contract Contract { get; set; } = null!;
}

public enum AcctiveStatus
{
    Acctive,
    Block,
    Warning,
    UnAccepted
}

public enum BusinessType
{
    Restaurant,
    CloudKitchen,
    StreetFood
}

public enum Role
{
    Owner,
    Partner,
    Manger
}
