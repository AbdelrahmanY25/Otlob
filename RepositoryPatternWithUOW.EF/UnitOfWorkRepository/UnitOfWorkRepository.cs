using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.EF.BaseRepository;
using Otlob.EF.IBaseRepository;
using RepositoryPatternWithUOW.Core.Models;


namespace Otlob.EF.UnitOfWorkRepository
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public IBaseRepository<UserComplaint> UserComplaints { get; private set; }
        public IBaseRepository<Delivery> Deliveries { get; private set; }
        public IBaseRepository<Meal> Meals { get; private set; }
        public IBaseRepository<Order> Orders { get; private set; }
        public IBaseRepository<Restaurant> Restaurants { get; private set; }
        public IBaseRepository<Point> Points { get; private set; }
        public IBaseRepository<Address> Addresses { get; private set; }
        public IBaseRepository<OrderedMeals> OrderedMeals { get; private set; }
        public IBaseRepository<Cart> Carts { get; private set; }
        public IBaseRepository<MealsInOrder> MealsInOrder { get; private set; }
        public IBaseRepository<CartInOrder> CartInOrder { get; private set; }
        public IBaseRepository<ApplicationUser> Users { get; private set; }
        public void Dispose()
        {
            _applicationDbContext.Dispose();
        }
        public void SaveChanges()
        {
            _applicationDbContext.SaveChanges();
        }


        private readonly ApplicationDbContext _applicationDbContext;

        public UnitOfWorkRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            Restaurants = CreateRepository<Restaurant>();
            Orders = CreateRepository<Order>();
            Meals = CreateRepository<Meal>();
            Deliveries = CreateRepository<Delivery>();
            UserComplaints = CreateRepository<UserComplaint>();
            Points = CreateRepository<Point>();
            Addresses = CreateRepository<Address>();
            Carts = CreateRepository<Cart>();
            OrderedMeals = CreateRepository<OrderedMeals>();
            CartInOrder = CreateRepository<CartInOrder>();
            MealsInOrder = CreateRepository<MealsInOrder>();
            Users = CreateRepository<ApplicationUser>();
        }

        private IBaseRepository<T> CreateRepository<T>() where T : class
        {
            return new BaseRepository<T>(_applicationDbContext);
        }
    }
}
