namespace Otlob.EF.UnitOfWorkRepository;

public class UnitOfWorkRepository(ApplicationDbContext applicationDbContext) : IUnitOfWorkRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;

    private IBaseRepository<Meal> _meals = default!;
    private IBaseRepository<Order> _orders = default!;
    private IBaseRepository<Category> _categories = default!;
    private IBaseRepository<Restaurant> _restaurants = default!;
    private IBaseRepository<Address> _addresses = default!;
    private IBaseRepository<CartDetails> _cartDetails = default!;
    private IBaseRepository<Cart> _carts = default!;
    private IBaseRepository<OrderDetails> _orderDetails = default!;
    private IBaseRepository<ApplicationUser> _users = default!;
    private IBaseRepository<MealPriceHistory> _mealsPriceHistories = default!;
    private IBaseRepository<TempOrder> _tempOrders = default!;


    public IBaseRepository<Meal> Meals 
    { 
        get 
        {
            _meals ??= new BaseRepository<Meal>(_context);

            return _meals;
        }
    }

    public IBaseRepository<Order> Orders
    { 
        get 
        {
            _orders ??= new BaseRepository<Order>(_context);
           
            return _orders;
        }
    }

    public IBaseRepository<Category> Categories
    {
        get
        {
            _categories ??= new BaseRepository<Category>(_context);
            
            return _categories;
        }
    }

    public IBaseRepository<Restaurant> Restaurants
    { 
        get 
        {
            _restaurants ??= new BaseRepository<Restaurant>(_context);
            
            return _restaurants;
        }
    }

    public IBaseRepository<Address> Addresses
    { 
        get 
        {
            _addresses ??= new BaseRepository<Address>(_context);
            
            return _addresses;
        }
    }

    public IBaseRepository<CartDetails> CartDetails
    { 
        get 
        {
            _cartDetails ??= new BaseRepository<CartDetails>(_context);
            
            return _cartDetails;
        }
    }

    public IBaseRepository<Cart> Carts
    { 
        get 
        {
            _carts ??= new BaseRepository<Cart>(_context);
            
            return _carts;
        }
    }

    public IBaseRepository<OrderDetails> OrderDetails
    { 
        get 
        {
            _orderDetails ??= new BaseRepository<OrderDetails>(_context);
            
            return _orderDetails;
        }
    }

    public IBaseRepository<ApplicationUser> Users
    { 
        get 
        {
            _users ??= new BaseRepository<ApplicationUser>(_context);
            
            return _users;
        }
    }

    public IBaseRepository<MealPriceHistory> MealsPriceHistories
    { 
        get 
        {
            _mealsPriceHistories ??= new BaseRepository<MealPriceHistory>(_context);

            return _mealsPriceHistories;
        }
    }

    public IBaseRepository<TempOrder> TempOrders
    { 
        get 
        {
            
            _tempOrders ??= new BaseRepository<TempOrder>(_context);

            return _tempOrders;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
