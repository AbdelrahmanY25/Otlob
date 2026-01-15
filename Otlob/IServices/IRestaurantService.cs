namespace Otlob.IServices;

public interface IRestaurantService
{    
    IQueryable<AcctiveRestaurantResponse>? GetAcctiveRestaurants();

    IQueryable<PendingRestaurantResponse>? GetUnAcceptedAndPendingRestaurants();

    PendingRestaurantResponse GetUnAcceptedRestaurant();

    PendingRestaurantResponse GetPendingRestaurant();

    Result<RestaurantDetailsResponse> GetRestaurantDetailsById(string id, bool isAvaliable);

    IEnumerable<DeletedRestauraantsResponse> GetDeletedRestaurants();
    
    Result DelteRestaurant(string restaurantKey);
    
    Result UnDelteRestaurant(string restaurantKey);

    AcctiveStatus GetRestaurantStatusById(int restaurantId);
    
    Result IsRestaurantIdExists(int restaurantId);
    
    int HowManyBranchesExistForRestaurant(int restaurantId);

    IEnumerable<SelectListItem> GetAllRestaurantsForDropdown();
}
