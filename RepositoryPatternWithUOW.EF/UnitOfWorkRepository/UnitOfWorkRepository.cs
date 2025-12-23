namespace Otlob.EF.UnitOfWorkRepository;

public class UnitOfWorkRepository(ApplicationDbContext applicationDbContext) : IUnitOfWorkRepository
{
    private readonly ApplicationDbContext _context = applicationDbContext;

    private IBaseRepository<ApplicationUser> _users = default!;
    private IBaseRepository<Order> _orders = default!;
    private IBaseRepository<Category> _categories = default!;
    private IBaseRepository<Restaurant> _restaurants = default!;
    private IBaseRepository<Address> _addresses = default!;
    private IBaseRepository<CartDetails> _cartDetails = default!;
    private IBaseRepository<Cart> _carts = default!;
    private IBaseRepository<OrderDetails> _orderDetails = default!;
    private IBaseRepository<MealAddOn> _mealAddOn = default!;
    private IBaseRepository<MealOptionGroup> _mealOptionGroups = default!;
    private IBaseRepository<MealOptionItem> _mealOptionItems = default!;
    private IBaseRepository<Meal> _meals = default!;
    private IBaseRepository<MealPriceHistory> _mealsPriceHistories = default!;
    private IBaseRepository<MenuCategory> _mealCategories = default!;
    private IBaseRepository<ManyMealManyAddOn> _manyMealManyAddOns = default!;
    private IBaseRepository<TempOrder> _tempOrders = default!;
    private IBaseRepository<TradeMark> _tradeMarks = default!;
    private IBaseRepository<UploadedFile> _uploadedFiles = default!;
    private IBaseRepository<CommercialRegistration> _commercialRegistrations = default!;
    private IBaseRepository<VAT> _vats = default!;
    private IBaseRepository<BankAccount> _bankAccounts = default!;
    private IBaseRepository<NationalId> _nationalIds = default!;
    private IBaseRepository<RestaurantBranch> _restaurantBranches = default!;
    private IBaseRepository<RestaurantCategory> _restaurantCategorys = default!;


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

    public IBaseRepository<MealAddOn> MealAddOns
    { 
        get 
        {
            _mealAddOn ??= new BaseRepository<MealAddOn>(_context);
            return _mealAddOn;
        }
    }

    public IBaseRepository<MealOptionGroup> MealOptionGroups
    { 
        get 
        {
            _mealOptionGroups ??= new BaseRepository<MealOptionGroup>(_context);
            return _mealOptionGroups;
        }
    }

    public IBaseRepository<MealOptionItem> MealOptionItems
    { 
        get 
        {
            _mealOptionItems ??= new BaseRepository<MealOptionItem>(_context);
            return _mealOptionItems;
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

    public IBaseRepository<MenuCategory> MealCategories
    {
        get
        {
            _mealCategories ??= new BaseRepository<MenuCategory>(_context);
            return _mealCategories;
        }
    }
    
    public IBaseRepository<ManyMealManyAddOn> ManyMealManyAddOns
    { 
        get 
        {
            _manyMealManyAddOns ??= new BaseRepository<ManyMealManyAddOn>(_context);
            return _manyMealManyAddOns;
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

    public IBaseRepository<UploadedFile> UploadedFiles
    {
        get
        {
            _uploadedFiles ??= new BaseRepository<UploadedFile>(_context);
            return _uploadedFiles;
        }
    }

    public IBaseRepository<CommercialRegistration> CommercialRegistrations
    {
        get
        {
            _commercialRegistrations ??= new BaseRepository<CommercialRegistration>(_context);
            return _commercialRegistrations;
        }
    }

    public IBaseRepository<TradeMark> TradeMarks
    {
        get
        {
            _tradeMarks ??= new BaseRepository<TradeMark>(_context);
            return _tradeMarks;
        }
    }

    public IBaseRepository<VAT> Vats
    {
        get
        {
            _vats ??= new BaseRepository<VAT>(_context);
            return _vats;
        }
    }

    public IBaseRepository<BankAccount> BankAccounts
    {
        get
        {
            _bankAccounts ??= new BaseRepository<BankAccount>(_context);
            return _bankAccounts;
        }
    }

    public IBaseRepository<NationalId> NationalIds
    {
        get
        {
            _nationalIds ??= new BaseRepository<NationalId>(_context);
            return _nationalIds;
        }
    }

    public IBaseRepository<RestaurantBranch> RestaurantBranches
    {
        get
        {
            _restaurantBranches ??= new BaseRepository<RestaurantBranch>(_context);
            return _restaurantBranches;
        }
    }

    public IBaseRepository<RestaurantCategory> RestaurantCategories
    {
        get
        {
            _restaurantCategorys ??= new BaseRepository<RestaurantCategory>(_context);
            return _restaurantCategorys;
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
