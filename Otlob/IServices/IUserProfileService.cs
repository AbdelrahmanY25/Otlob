namespace Otlob.IServices;

public interface IUserProfileService
{
    Task<Result<UserProfile>> GetUserProfileDetails(string userId);
    Task<Result> UpdateUserProfileAsync(UserProfile request, string userId);
    Task<Result> UpdateUserProfilePictureAsync(string userId, IFormFile image);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest changePsswordRequest);
    Task<Result> ChangeUserPasswordAsync(string userId, string newPassword);
    Task LogOutAsync();
}
