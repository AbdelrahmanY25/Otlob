using Otlob.Core.Models;
using Otlob.EF.IBaseRepository;
using RepositoryPatternWithUOW.Core.Models;

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
        IBaseRepository<CartInOrder> CartInOrder { get; }
        IBaseRepository<MealsInOrder> MealsInOrder { get; }
        IBaseRepository<ApplicationUser> Users { get; }
        void SaveChanges();
    }
}
