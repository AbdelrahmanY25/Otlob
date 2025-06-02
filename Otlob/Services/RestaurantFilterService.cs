namespace Otlob.Services
{
    public class RestaurantFilterService : IRestaurantFilterService
    {       
        public Expression<Func<Restaurant, bool>> RestaurantCategoryFilter(RestaurantCategory? filter = null)
        {
            if (filter is null || filter == RestaurantCategory.All)
            {
                return r => true;
            }

            return r => r.Category == filter;
        }

        public Expression<Func<Restaurant, bool>> RestaurantsStatusFilter(AcctiveStatus[]? statuses = null)
        {
            if (statuses is null || statuses.Length == 0)
            {
                return r => r.AcctiveStatus == AcctiveStatus.Acctive || r.AcctiveStatus == AcctiveStatus.Warning;
            }

            return r => statuses.Contains(r.AcctiveStatus);
        }

        public bool ValidateDataWhenEditRedtaurantProfile(RestaurantVM restaurantVM, Restaurant oldResturantInfo)
        {
            return restaurantVM.Name == oldResturantInfo?.Name &&
                   restaurantVM.Email == oldResturantInfo.Email &&
                   restaurantVM.Address == oldResturantInfo.Address;
        }
    }
}
