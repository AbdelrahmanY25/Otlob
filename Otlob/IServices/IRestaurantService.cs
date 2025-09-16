namespace Otlob.IServices;

public interface IRestaurantService
{
    Task<Result<int>> GetRestaurantIdByUserId(string userId);
    
    Result<RestaurantVM> GetRestaurant(int restaurantId);
    
    IQueryable<RestaurantVM>? GetAllRestaurants(Category? filter, AcctiveStatus[]? statuses = null);  

    Result<RestaurantVM> GetRestaurantDetailsById(int restaurantId);      
    
    IQueryable<RestaurantVM>? GetDeletedRestaurants();
    
    Task<bool> DelteRestaurant(string id);
    
    Task<bool> UnDelteRestaurant(string id);
}
