namespace Otlob.IServices;

public interface IRestaurantService
{    
    Result<RestaurantVM> GetRestaurant(int restaurantId);
    
    IQueryable<RestaurantVM>? GetAllRestaurants(Category? filter, AcctiveStatus[]? statuses = null);

    IQueryable<PendingRestaurantResponse>? GetUnAcceptedAndPendingRestaurants();

    PendingRestaurantResponse GetUnAcceptedRestaurant();

    PendingRestaurantResponse GetPendingRestaurant();

    Result<RestaurantDetailsResponse> GetRestaurantDetailsById(string id);      
    
    IQueryable<RestaurantVM>? GetDeletedRestaurants();
    
    Task<bool> DelteRestaurant(string id);
    
    Task<bool> UnDelteRestaurant(string id);

    AcctiveStatus GetRestaurantStatusById(int restaurantId);
    
    Result IsRestaurantIdExists(int restaurantId);
    
    int HowManyBranchesExistForRestaurant(int restaurantId);
}
