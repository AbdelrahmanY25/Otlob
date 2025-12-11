namespace Otlob.Services;

public class AuthService(IMapper mapper, SignInManager<ApplicationUser> signInManager,
                        UserManager<ApplicationUser> userManager, ISendEmailsToUsersService sendEmailsToUsersService,
                        IHttpContextAccessor httpContextAccessor,
                        ISendEmailsToPartnersService sendEmailsToPartnersService, IUnitOfWorkRepository unitOfWorkRepository) : IAuthService
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly ISendEmailsToUsersService _sendEmailsToUsersService = sendEmailsToUsersService;
    private readonly ISendEmailsToPartnersService _sendEmailsToPartnersService = sendEmailsToPartnersService;

    public async Task<Result> RegisterAsync(RegisterRequest request, List<string> roles)
    {
        var isExists = await IsValidUserData(request);

        if (isExists.IsFailure)
        {
            return Result.Failure(isExists.Error);
        }

        ApplicationUser newUser = _mapper.Map<ApplicationUser>(request);

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }        

        var sendEmailResult = await SendEmailConfirmationAsync(newUser);

        if (sendEmailResult.IsFailure)
        {
            return Result.Failure(sendEmailResult.Error);
        }

        await _userManager.AddToRolesAsync(newUser, roles);

        return Result.Success();
    }
    
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {        
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure(AuthenticationErrors.DoublicatedConfirmation);
        }

        string token = request.Token;

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration("Invalid Token"));
        }

        var result = await _userManager.ConfirmEmailAsync(user!, token);

        if (!result.Succeeded)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        await SendWelcomeEmailToUserBasedOnHisRoleAsync(user);        

        return Result.Success();
    }

    public async Task<Result> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return Result.Failure(AuthenticationErrors.InvalidEmail);
        }

        var result = await SendEmailConfirmationAsync(user);

        return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request)
    {
        var result = await IsValidUser(request.Email);

        if (result.IsFailure)
        {
            return Result.Failure<string>(result.Error);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        var signInResults = await _signInManager.PasswordSignInAsync(user!.UserName!, request.Password, request.RememberMe, true);

        if (!signInResults.Succeeded)
        {
            return Result.Failure<string>(signInResults.IsNotAllowed ? 
                AuthenticationErrors.NoEmailConfirmed : AuthenticationErrors.InvalidCredentials);
        }        

        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        await _signInManager.SignInWithClaimsAsync(user, request.RememberMe, await AddUserClaimsAsync(user, userRole));        

        return Result.Success(userRole)!;
    }

    public async Task<Result> ForgetPasswordAsync(ForgetPasswordRequest request)
    {
        var result = await IsValidUser(request.Email);

        if (result.IsFailure)
        {
            return Result.Success();
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (!user!.EmailConfirmed)
        {
            return Result.Failure(AuthenticationErrors.NoEmailConfirmed);
        }
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user!);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var origin = _httpContextAccessor.HttpContext!.Request.Headers.Origin;

        var callbackUrl = $"{origin}/Customer/Account/ResetPassword?email={user!.Email}&token={token}";

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.WhenForgetHisPasswordAsync(callbackUrl!, user!));

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var validationResult = await IsValidUser(request.Email);

        if (validationResult.IsFailure)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration("Invalid Token"));
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (!user!.EmailConfirmed)
        {
            return Result.Failure(AuthenticationErrors.NoEmailConfirmed);
        }

        string token = request.Token;

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration("Invalid Token"));
        }

        var result = await _userManager.ResetPasswordAsync(user!, token, request.NewPassword!);

        if (!result.Succeeded)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
    {
        string userId = _httpContextAccessor.HttpContext!.User.GetUserId();

        if (userId.IsNullOrEmpty())
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }        

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.WhenCahngeHisPasswordAsync(user));

        return Result.Success();
    }

    public async Task LogOutAsync() => 
        await _signInManager.SignOutAsync();




    private async Task<Result> SendEmailConfirmationAsync(ApplicationUser user)
    {
        if (user.EmailConfirmed)
        {
            return Result.Failure(AuthenticationErrors.DoublicatedConfirmation);
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var callbackUrl = $"{origin}/Customer/Account/ConfirmEmail?userId={user.Id}&token={token}";

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.ConfirmEmailAsync(callbackUrl, user));

        return Result.Success();
    }

    private async Task SendWelcomeEmailToUserBasedOnHisRoleAsync(ApplicationUser user)
    {
        string userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;

        if (userRole == DefaultRoles.Customer)
        {
            BackgroundJob.Enqueue(() => _sendEmailsToUsersService.WhenCreateUserAccountAsync(user));
        }
        else if (userRole == DefaultRoles.RestaurantAdmin)
        {
            string restaurantName = _unitOfWorkRepository.Restaurants.GetOneWithSelect(
                expression: r => r.OwnerId == user.Id,
                tracked: false,
                selector: r => r.Name
            )!;

            BackgroundJob.Enqueue(() => _sendEmailsToPartnersService.WhenCreatePartnerAccountAsync(user, restaurantName));
        }
    }

    private async Task<List<Claim>> AddUserClaimsAsync(ApplicationUser user, string? userRole)
    {
        List<Claim> claims = [];

        if (userRole is DefaultRoles.RestaurantAdmin)
        {

            int restaurantId = _unitOfWorkRepository.Restaurants
                .GetOneWithSelect(
                    expression: r => r.OwnerId == user.Id,
                    tracked: false,
                    selector: r => r.Id
                );

            claims.Add(new Claim(StaticData.RestaurantId, restaurantId.ToString()));
        }

        if (user.Image is not null)
            claims.Add(new Claim(StaticData.UserImageProfile, user.Image!));

        return claims;
    }

    private async Task<Result> IsValidUserData(RegisterRequest request)
    {
        bool isUserNameExist = await _userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.UserName == request.UserName);

        if (isUserNameExist)
            return Result.Failure(AuthenticationErrors.DoublicatedUserName(request.UserName));

        bool isEmailExist = await _userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);

        if (isEmailExist)
            return Result.Failure(AuthenticationErrors.DoublicatedEmail(request.Email));

        bool isPhoneNumberExist = await _userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.PhoneNumber == request.PhoneNumber);
        
        if (isPhoneNumberExist) 
            return Result.Failure(AuthenticationErrors.DoublicatedPhoneNumber);           

        return Result.Success();
    }

    private async Task<Result> IsValidUser(string email)
    {
        bool isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == email);

        if (!isEmailExists)
        {
            return Result.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var user = await _userManager.Users
          .AsNoTracking()
          .Where(u => u.Email == email)
          .Select(selector: u => new ApplicationUser
          {
              LockoutEnabled = u.LockoutEnabled,
              LockoutEnd = u.LockoutEnd
          })
          .FirstOrDefaultAsync();

        bool isUserLockoutEnabled = user!.LockoutEnabled;

        if (!isUserLockoutEnabled)
        {
            return Result.Failure(AuthenticationErrors.LockedOutUser);
        }

        var userLockoutEndDate = user.LockoutEnd;

        if (userLockoutEndDate.HasValue && userLockoutEndDate.Value > DateTimeOffset.UtcNow)
        {
            return Result.Failure(AuthenticationErrors.UserLockoutEndDate(userLockoutEndDate.Value.LocalDateTime));
        }

        return Result.Success();
    }
}
