using System.Data;

namespace Otlob.Core.IUnitOfWorkRepository;

public interface IUnitOfWorkRepository : IDisposable
{
    IBaseRepository<Category> Categories { get; }
    IBaseRepository<Restaurant> Restaurants { get; }
    IBaseRepository<Address> Addresses { get; }
    IBaseRepository<Order> Orders { get; }
    IBaseRepository<CartDetails> CartDetails { get; }
    IBaseRepository<Cart> Carts { get; }
    IBaseRepository<OrderDetails> OrderDetails { get; }
    IBaseRepository<ApplicationUser> Users { get; }
    IBaseRepository<MealAddOn> MealAddOns { get; }
    IBaseRepository<MealOptionGroup> MealOptionGroups { get; }
    IBaseRepository<MealOptionItem> MealOptionItems { get; }
    IBaseRepository<Meal> Meals { get; }
    IBaseRepository<MealPriceHistory> MealsPriceHistories { get; }
    IBaseRepository<MenuCategory> MealCategories { get; }
    IBaseRepository<ManyMealManyAddOn> ManyMealManyAddOns { get; }
    IBaseRepository<TempOrder> TempOrders { get; }
    IBaseRepository<TradeMark> TradeMarks { get; }
    IBaseRepository<UploadedFile> UploadedFiles { get; }
    IBaseRepository<CommercialRegistration> CommercialRegistrations { get; }
    IBaseRepository<VAT> Vats { get; }
    IBaseRepository<BankAccount> BankAccounts { get; }
    IBaseRepository<NationalId> NationalIds { get; }
    IBaseRepository<RestaurantBranch> RestaurantBranches { get; }
    IBaseRepository<RestaurantCategory> RestaurantCategories { get; }

    IDbTransaction BeginTransaction();
    void SaveChanges();
}
