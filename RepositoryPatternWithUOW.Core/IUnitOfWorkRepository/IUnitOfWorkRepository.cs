using Otlob.Core.Entities;
using System.Data;

namespace Otlob.Core.IUnitOfWorkRepository;

public interface IUnitOfWorkRepository : IDisposable
{
    IBaseRepository<AdminDailyAnalytic> AdminDailyAnalytics { get; }
    IBaseRepository<AdminMonthlyAnalytic> AdminMonthlyAnalytics { get; }
    IBaseRepository<Category> Categories { get; }
    IBaseRepository<Restaurant> Restaurants { get; }
    IBaseRepository<RestaurantRatingAnlytic> RestaurantRatingAnlytics { get; }
    IBaseRepository<RestaurantDailyAnalytic> RestaurantDailyAnalytics { get; }
    IBaseRepository<RestaurantMonthlyAnalytic> RestaurantMonthlyAnalytics { get; }
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
    IBaseRepository<MealsAnalytic> MealsAnalytics { get; }
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
    IBaseRepository<OrderRating> OrderRatings { get; }
    IBaseRepository<PromoCode> PromoCodes { get; }
    IBaseRepository<PromoCodeUsage> PromoCodeUsages { get; }

    IDbTransaction BeginTransaction();
    void SaveChanges();
}
