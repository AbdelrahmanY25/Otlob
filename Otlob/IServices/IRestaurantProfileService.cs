namespace Otlob.IServices;

public interface IRestaurantProfileService
{
    Result EditRestaurantProfileInfo(RestaurantProfile request, int restaurantId);
    
    Result EditRestaurantProfilePicture(int restaurantId, IFormFile image);

    Result<RestaurantProfile> GetRestaurantProfileDetailsById(int restaurantId);
}