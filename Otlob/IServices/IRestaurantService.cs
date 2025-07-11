namespace Otlob.Core.IServices
{
    public interface IRestaurantService
    {
        IQueryable<RestaurantVM> GetAllRestaurantsJustMainInfo(RestaurantCategory? filter, AcctiveStatus[]? statuses = null);
        Restaurant GetRestaurantImageById(int restaurantId);
        RestaurantVM GetRestaurantJustMainInfo(int restaurantId);
        RestaurantVM GetRestaurantVMDetailsById(int restaurantId);
        string EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId, bool ValidateData = true);
        void UpdateRestaurantImage(Restaurant restaurant, string imageUrl);
        bool ChangeRestauranStatus(string id, AcctiveStatus status);
        IQueryable<RestaurantVM>? GetDeletedRestaurants();
        bool DelteRestaurant(string id);
        bool UnDelteRestaurant(string id);
    }
}
