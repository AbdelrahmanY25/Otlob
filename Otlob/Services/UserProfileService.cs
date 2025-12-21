namespace Otlob.Services;

public class UserProfileService(UserManager<ApplicationUser> userManager, IFileService imageService) : IUserProfileService
{
    private readonly IFileService _imageService = imageService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<UserProfile>> GetUserProfileDetails(string userId)
    {
        UserProfile response = await _userManager.Users
            .Where(u => u.Id == userId)
            .AsNoTracking()
            .Select(
                u => new UserProfile
                {
                    UserName = u.UserName!,
                    Email = u.Email!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    BirthDate = u.BirthDate,
                    PhoneNumber = u.PhoneNumber!,
                    Image = u.Image
                }
             )
            .SingleAsync();

        if (response is null)
            return Result.Failure<UserProfile>(AuthenticationErrors.InvalidUser);

        return Result.Success(response);
    }

    public async Task<Result> UpdateUserProfileAsync(UserProfile request, string userId)
    {
        Result isValidPhone = await ValidateUserProfileForUpdateAsync(request, userId);
        
        if (isValidPhone.IsFailure)
            return Result.Failure(AuthenticationErrors.DoublicatedPhoneNumber);

        await _userManager.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(u => u.FirstName, request.FirstName)
                   .SetProperty(u => u.LastName, request.LastName)
                   .SetProperty(u => u.PhoneNumber, request.PhoneNumber)
                   .SetProperty(u => u.BirthDate, request.BirthDate)
                   .SetProperty(u => u.Gender, request.Gender)
            );

        return Result.Success();
    }    

    public async Task<Result> UpdateUserProfilePictureAsync(string userId, IFormFile image)
    {
       var isImageUploaded = _imageService.UploadImage(image!);

        if (isImageUploaded.IsFailure)
        {
            return isImageUploaded;
        }

        string? userImage = await _userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Image!)
            .FirstOrDefaultAsync();

        var isOldImageDeleted = _imageService.DeleteImage(userImage);

        if (isOldImageDeleted.IsFailure)
        {
            _imageService.DeleteImage(isImageUploaded.Value);
            return isOldImageDeleted;
        }

        await _userManager.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(u => u.Image, isImageUploaded.Value)
            );

        return Result.Success(isImageUploaded.Value);
    }

    private async Task<Result> ValidateUserProfileForUpdateAsync(UserProfile request, string userId)
    {
        bool isPhoneNumberExists = await _userManager.Users
                    .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId);

        if (isPhoneNumberExists)
            return Result.Failure(AuthenticationErrors.DoublicatedPhoneNumber);

        return Result.Success();
    }
}
