namespace Otlob.IServices;

public interface IUserProfileService
{
    Task<Result<ProfileVM>> GetUserProfileVmDetails(string userId);
    Task<Result> UpdateUserProfileAsync(ProfileVM profileVM, string userId);
    Task<Result> UpdateUserProfilePictureAsync(string userId, IFormFile image);
}
