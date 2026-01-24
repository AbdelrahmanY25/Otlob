namespace Otlob.ApiServices;

public interface IApiUserProfileService
{
    Task<ApiResult<UserProfileResponse>> GetUserProfileAsync();
    Task<ApiResult> UpdateUserProfileAsync(UpdateUserProfileRequest request);
    Task<ApiResult> DeleteAccountAsync();
    Task<ApiResult<string>> UpdateProfilePictureAsync(IFormFile image);
}
