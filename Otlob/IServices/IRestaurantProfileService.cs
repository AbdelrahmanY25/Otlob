namespace Otlob.IServices;

public interface IRestaurantProfileService
{
    Result EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId);
    
    Result EditRestaurantProfilePicture(int restaurantId, IFormFile image);   
    
    Result<RestaurantVM> GetRestaurantProfileDetailsById(int restaurantId);
}