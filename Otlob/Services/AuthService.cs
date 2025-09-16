namespace Otlob.Services;

public class AuthService(IMapper mapper, SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager, ISendEmailsToUsersService sendEmailsToUsersService, IRestaurantService restaurantService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly IMapper _mapper = mapper;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ISendEmailsToUsersService _sendEmailsToUsersService = sendEmailsToUsersService;
    private readonly IRestaurantService _restaurantService = restaurantService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<string>> RegisterAsync(ApplicationUserVM userVM, List<string> roles)
    {
        ApplicationUser newUser = _mapper.Map<ApplicationUser>(userVM);

        var result = await _userManager.CreateAsync(newUser, userVM.Password);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        await _userManager.AddToRolesAsync(newUser, roles);

        await _signInManager.SignInAsync(newUser, false);

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.WhenCreateAccountAsync(newUser));

        return Result.Success(newUser.Id);
    }
    
    public async Task<Result<string>> LoginAsync(LoginVM loginVM)
    {
        var user = await _userManager.FindByEmailAsync(loginVM.Email);

        var finalResult = await _userManager.CheckPasswordAsync(user!, loginVM.Password);

        if (!finalResult)
        {
            await _userManager.AccessFailedAsync(user!);

            return Result.Failure<string>(AuthenticationErrors.InvalidCredintials);
        }

        if (user!.AccessFailedCount > 0)
        {
            await _userManager.ResetAccessFailedCountAsync(user!);
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var userRole = userRoles.FirstOrDefault();

        await _signInManager.SignInWithClaimsAsync(user, loginVM.RememberMe, await AddUserClaimsAsync(user, userRole));

        return Result.Success(userRole)!;
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordVM model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        var result = await _userManager.ResetPasswordAsync(user!, model.Token, model.NewPassword!);

        if (!result.Succeeded)
        {
            return Result.Failure(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        return Result.Success(true);
    }

    public async Task<Result<string>> ChangePasswordAsync(ChangePasswordVM changePsswordVM)
    {
        string userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (userId.IsNullOrEmpty())
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidUser);
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidUser);
        }

        bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePsswordVM.OldPassword!);

        if (!isOldPasswordValid || changePsswordVM.OldPassword == changePsswordVM.NewPassword || changePsswordVM.NewPassword != changePsswordVM.ConfirmNewPassword)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidPasswordChecked);
        }

        var result = await _userManager.ChangePasswordAsync(user, changePsswordVM.OldPassword, changePsswordVM.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(AuthenticationErrors.InvalidRegistration(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.WhenCahngeHisPasswordAsync(user));

        return Result.Success("Your Password updated successfully.");
    }

    public async Task LogOutAsync() => 
        await _signInManager.SignOutAsync();

    private async Task<List<Claim>> AddUserClaimsAsync(ApplicationUser user, string? userRoles)
    {
        List<Claim> claims = [];

        if (userRoles is not null && userRoles == SD.restaurantAdmin)
        {
            var result = await _restaurantService.GetRestaurantIdByUserId(user.Id);

            int restaurantId = result.Value;
            claims.Add(new Claim(SD.restaurantId, restaurantId.ToString()));
        }

        if (user.Image is not null)
            claims.Add(new Claim(SD.userImageProfile, user.Image!));

        return claims;
    }
}
