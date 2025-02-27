using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Core.IServices
{
    public interface IRestaurantService
    {
        IQueryable<RestaurantVM> GetAllRestaurantsJustMainInfo(RestaurantCategory? filter, AcctiveStatus[]? statuses = null);
        RestaurantVM GetRestaurantJustMainInfo(int restaurantId);
        RestaurantVM GetRestaurant(int restaurantId);
        Task<string>? EditRestaurantProfileInfo(RestaurantVM restaurantVM, int restaurantId, IFormFileCollection image, bool ValidateData = true);
        bool ChangeRestauranStatus(int restaurantId, AcctiveStatus status);
    }
}
