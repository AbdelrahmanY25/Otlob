using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Core.IServices
{
    public interface IUserServices
    {
        ApplicationUser UpdateUserInfo(ApplicationUser user, ProfileVM profileVM);
        ApplicationUser UpdateRestaurantAdminInfo(ProfileVM profileVM, int restaurantId);
        ProfileVM ViewUserProfileVmDetails(string? userId, int? restaurantId = null);
    }
}
