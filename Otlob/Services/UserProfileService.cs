namespace Otlob.Services;

public class UserProfileService(UserManager<ApplicationUser> userManager, IMapper mapper,
                                IImageService imageService, IUnitOfWorkRepository unitOfWorkRepository) : IUserProfileService
{
    private readonly IMapper _mapper = mapper;
    private readonly IImageService _imageService = imageService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public async Task<Result<ProfileVM>> GetUserProfileVmDetails(string userId)
    {
        if (userId is null)
        {
            return Result.Failure<ProfileVM>(AuthenticationErrors.InvalidUser);
        }

        bool isUserIdExist = await _userManager.Users.AnyAsync(u => u.Id == userId);

        if (!isUserIdExist)
        {
            return Result.Failure<ProfileVM>(AuthenticationErrors.InvalidUser);
        }

        ProfileVM userProfile = _unitOfWorkRepository.Users
           .GetOneWithSelect
            (
                expression: u => u.Id == userId,
                tracked: false,
                selector: u => new ProfileVM
                {
                    Email = u.Email!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    BirthDate = u.BirthDate,
                    PhoneNumber = u.PhoneNumber!,
                    Image = u.Image
                }
            )!;

        return Result.Success(userProfile);
    }

    public async Task<Result> UpdateUserProfileAsync(ProfileVM profileVM, string userId)
    {
        if (userId is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        bool isUserIdExist = await _userManager.Users.AnyAsync(u => u.Id == userId);

        if (!isUserIdExist)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        ApplicationUser user = (await _userManager.FindByIdAsync(userId))!;

        bool noChanges = ThereIsNewData(user!, profileVM);

        if (noChanges)
        {
            return Result.Failure(AuthenticationErrors.NoNewData);
        }

        Result validData = await ValidateOnUserData(user, profileVM);

        if (validData.IsFailure)
        {
            return validData;
        }

        _mapper.Map(profileVM, user);

        var updateResult = await _userManager.UpdateAsync(user!);

        if (!updateResult.Succeeded)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration(string.Join(", ", updateResult.Errors.Select(e => e.Description))));
        }

        return Result.Success();
    }

    public async Task<Result> UpdateUserProfilePictureAsync(string userId, IFormFile image)
    {
        if (userId is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        bool isUserIdExist = await _userManager.Users.AnyAsync(u => u.Id == userId);

        if (!isUserIdExist)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        var isImageUploaded = _imageService.UploadImage(image!);

        if (isImageUploaded.IsFailure)
        {
            return isImageUploaded;
        }

        ApplicationUser user = (await _userManager.FindByIdAsync(userId))!;

        var isOldImageDeleted = _imageService.DeleteImageIfExist(user.Image!);

        if (isOldImageDeleted.IsFailure)
        {
            _imageService.DeleteImageIfExist(isImageUploaded.Value);
            return isOldImageDeleted;
        }

        user.Image = isImageUploaded.Value;

        _unitOfWorkRepository.Users.ModifyProperty(user, u => u.Image!);

        _unitOfWorkRepository.SaveChanges();

        return Result.Success(isImageUploaded.Value);
    }


    private static bool ThereIsNewData(ApplicationUser user, ProfileVM profileVM)
    {
        return profileVM.Email == user.Email && profileVM.FirstName == user.FirstName &&
                profileVM.LastName == user.LastName && profileVM.Gender == user.Gender &&
                profileVM.PhoneNumber == user.PhoneNumber && profileVM.BirthDate == user.BirthDate;
    }

    private async Task<Result> ValidateOnUserData(ApplicationUser user, ProfileVM profileVM)
    {
        bool isEmailExist = await _userManager.Users.AnyAsync(u => u.Email == profileVM.Email);

        if (isEmailExist && user!.Email != profileVM.Email)
        {
            return Result.Failure(AuthenticationErrors.InvalidUserEmail(profileVM.Email));
        }

        return Result.Success();
    }
}
