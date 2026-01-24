namespace Otlob.IServices;

public interface IRestaurantProfileService
{
    Result<RestaurantProfile> GetRestaurantProfileDetailsById(int restaurantId);
    Result EditRestaurantProfileInfo(RestaurantProfile request, int restaurantId);    
    Result EditRestaurantProfilePicture(int restaurantId, IFormFile image);
    Result EditRestaurantProfileCover(int restaurantId, IFormFile image);
}