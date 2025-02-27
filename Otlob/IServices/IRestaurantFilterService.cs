using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using System.Linq.Expressions;

namespace Otlob.IServices
{
    public interface IRestaurantFilterService
    {
        Expression<Func<Restaurant, bool>> RestaurantCategoryFilter(RestaurantCategory? filter = null);
        Expression<Func<Restaurant, bool>> RestaurantsStatusFilter(AcctiveStatus[]? statuses = null);
        bool ValidateDataWhenEditRedtaurantProfile(RestaurantVM restaurantVM, Restaurant oldResturantInfo);
    }
}
