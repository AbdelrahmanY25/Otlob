namespace Otlob.Services;

public class FavouritesService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider,
                               IHttpContextAccessor httpContextAccessor) : IFavouritesService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public bool Toggle(string restaurantKey)
    {
        var userId = GetCurrentUserId();
        
        if (string.IsNullOrEmpty(userId))
            return false;

        int restaurantId;
        
        try
        {
            restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));
        }
        catch
        {
            return false;
        }

        // Check if restaurant exists
        var restaurantExists = _unitOfWorkRepository.Restaurants
            .IsExist(r => r.Id == restaurantId);

        if (!restaurantExists)
            return false;

        // Check if already in favourites
        var existingFavourite = _unitOfWorkRepository.UsersFavourites
            .GetOne(expression: f => f.UserId == userId && f.RestaurantId == restaurantId);

        if (existingFavourite is not null)
        {
            // Remove from favourites
            _unitOfWorkRepository.UsersFavourites.HardDelete(existingFavourite);
        }
        else
        {
            // Add to favourites
            var newFavourite = new UsersFavourites
            {
                UserId = userId,
                RestaurantId = restaurantId
            };
            _unitOfWorkRepository.UsersFavourites.Add(newFavourite);
        }

        _unitOfWorkRepository.SaveChanges();
        
        return true;
    }

    public IEnumerable<FavouritesResponse> GetFavoritesList()
    {
        var userId = GetCurrentUserId();
        
        if (string.IsNullOrEmpty(userId))
            return [];

        var favourites = _unitOfWorkRepository.UsersFavourites
            .GetAllWithSelect(
                expression: f => f.UserId == userId,
                tracked: false,
                selector: f => new FavouritesResponse
                {
                    RestaurantKey = _dataProtector.Protect(f.RestaurantId.ToString()),
                    RestaurantName = f.Restaurant.Name,
                    Image = f.Restaurant.Image,
                    CoverImage = f.Restaurant.CoverImage,
                    Rating = f.Restaurant.Rating,
                    RatesCount = f.Restaurant.RatingAnlytic != null ? f.Restaurant.RatingAnlytic.RatesCount : 0,
                    Categories = f.Restaurant.RestaurantCategories.Select(c => c.Category.Name),
                    DeliveryFee = f.Restaurant.DeliveryFee,
                    DeliveryTime = f.Restaurant.DeliveryDuration
                }
            );

        return favourites?.ToList() ?? [];
    }

    public bool IsFavorite(string restaurantKey)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return false;

        int restaurantId;
        try
        {
            restaurantId = int.Parse(_dataProtector.Unprotect(restaurantKey));
        }
        catch
        {
            return false;
        }

        return _unitOfWorkRepository.UsersFavourites
            .IsExist(f => f.UserId == userId && f.RestaurantId == restaurantId);
    }

    private string? GetCurrentUserId()
        => _httpContextAccessor.HttpContext?.User.GetUserId();
   
}
