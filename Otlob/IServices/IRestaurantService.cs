namespace Otlob.Core.IServices
{
    public interface IRestaurantService
    {
        IQueryable<RestaurantVM> GetAllRestaurantsJustMainInfo(RestaurantCategory? filter, AcctiveStatus[]? statuses = null);
        RestaurantVM GetRestaurantJustMainInfo(int restaurantId);
        RestaurantVM GetRestaurantDetailsById(int restaurantId);
        Task<string>? EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId, IFormFileCollection image, bool ValidateData = true);
        bool ChangeRestauranStatus(string id, AcctiveStatus status);
        IQueryable<Restaurant>? GetDeletedRestaurants();
        bool DelteRestaurant(string id);
        bool UnDelteRestaurant(string id);
    }
}
