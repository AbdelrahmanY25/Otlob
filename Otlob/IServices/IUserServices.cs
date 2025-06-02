namespace Otlob.Core.IServices
{
    public interface IUserServices
    {
        IQueryable<ApplicationUser>? GetAllUsers(Expression<Func<ApplicationUser, bool>>? query = null);
        void ChangeBlockUserStatus(string userId, bool blockstatus);
        ApplicationUser UpdateUserInfo(ApplicationUser user, ProfileVM profileVM);
        ApplicationUser UpdateUserProfile(ProfileVM profileVM, string userId);
        ProfileVM GetUserProfileVmDetails(string userId);
        ApplicationUser? GetUserDataToPartialview(string userId);
        int GetUserRestaurantId(string userId);
        string? GetUserIdByRestaurantId(int restaurantId);
    }
}
