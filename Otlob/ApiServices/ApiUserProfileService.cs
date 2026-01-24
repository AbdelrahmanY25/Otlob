namespace Otlob.ApiServices;

public class ApiUserProfileService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IFileService fileService) : IApiUserProfileService
{
    private readonly IFileService _fileService = fileService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ApiResult<UserProfileResponse>> GetUserProfileAsync()
    {
        string userId = _httpContextAccessor.HttpContext?.User.GetUserId()!;
        
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
            return ApiResult.Failure<UserProfileResponse>(UserProfileApiErrors.UserNotFound);
        
        var response = new UserProfileResponse
        (
            user.FirstName,
            user.LastName,
            user.Email,
            user.UserName,
            user.PhoneNumber,
            user.Gender,
            user.BirthDate,
            user.Image
        );
        
        return ApiResult.Success(response);
    }

    public async Task<ApiResult> UpdateUserProfileAsync(UpdateUserProfileRequest request)
    {
        string userId = _httpContextAccessor.HttpContext?.User.GetUserId()!;

        await _userManager.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(u => u.FirstName, request.FirstName)
                   .SetProperty(u => u.LastName, request.LastName)
                   .SetProperty(u => u.PhoneNumber, request.PhoneNumber)
                   .SetProperty(u => u.BirthDate, request.DateOfBirth)
                   .SetProperty(u => u.Gender, request.Gender)
            );

        return ApiResult.Success();
    }

    public async Task<ApiResult<string>> UpdateProfilePictureAsync(IFormFile image)
    {
        string userId = _httpContextAccessor.HttpContext?.User.GetUserId()!;
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
            return ApiResult.Failure<string>(UserProfileApiErrors.UserNotFound);

        if (image is null || image.Length == 0)
            return ApiResult.Failure<string>(UserProfileApiErrors.InvalidImage);

        // Delete old image if exists
        if (!string.IsNullOrEmpty(user.Image))
        {
            _fileService.DeleteImage(user.Image);
        }

        var uploadResult = _fileService.UploadImage(image);
        
        if (uploadResult.IsFailure)
            return ApiResult.Failure<string>(UserProfileApiErrors.InvalidImage);

        user.Image = uploadResult.Value;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            return ApiResult.Failure<string>(UserProfileApiErrors.AccountDeletionFailed);
        return ApiResult.Success(user.Image);
    }

    public async Task<ApiResult> DeleteAccountAsync()
    {
        string userId = _httpContextAccessor.HttpContext?.User.GetUserId()!;
        
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
            return ApiResult.Failure(UserProfileApiErrors.UserNotFound);
        
        var result = await _userManager.DeleteAsync(user);
        
        if (!result.Succeeded)
            return ApiResult.Failure(UserProfileApiErrors.AccountDeletionFailed);
        
        return ApiResult.Success();
    }
}
