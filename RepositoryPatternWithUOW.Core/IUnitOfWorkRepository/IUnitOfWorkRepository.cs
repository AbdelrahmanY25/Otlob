using Otlob.Core.Models;
using Otlob.EF.IBaseRepository;

namespace Otlob.Core.IUnitOfWorkRepository
{
    public interface IUnitOfWorkRepository : IDisposable
    {
        IBaseRepository<UserComplaint> UserComplaints { get; }
        IBaseRepository<Delivery> Deliveries { get; }
        IBaseRepository<Meal> Meals { get; }
        IBaseRepository<Order> Orders { get; }
        IBaseRepository<Restaurant> Restaurants { get; }
        IBaseRepository<Point> Points { get; }
        IBaseRepository<Address> Addresses { get; }
        IBaseRepository<OrderedMeals> OrderedMeals { get; }
        IBaseRepository<Cart> Carts { get; }
        IBaseRepository<MealsInOrder> MealsInOrder { get; }
        IBaseRepository<ApplicationUser> Users { get; }
        IBaseRepository<MealPriceHistory> MealsPriceHistories { get; }
        void SaveChanges();
    }
}
