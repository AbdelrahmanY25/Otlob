namespace Otlob.Core.IUnitOfWorkRepository;

public interface IUnitOfWorkRepository : IDisposable
{
    IBaseRepository<Category> Categories { get; }
    IBaseRepository<Restaurant> Restaurants { get; }
    IBaseRepository<Meal> Meals { get; }
    IBaseRepository<Address> Addresses { get; }
    IBaseRepository<Order> Orders { get; }
    IBaseRepository<CartDetails> CartDetails { get; }
    IBaseRepository<Cart> Carts { get; }
    IBaseRepository<OrderDetails> OrderDetails { get; }
    IBaseRepository<ApplicationUser> Users { get; }
    IBaseRepository<MealPriceHistory> MealsPriceHistories { get; }
    IBaseRepository<TempOrder> TempOrders { get; }

    void SaveChanges();
}
