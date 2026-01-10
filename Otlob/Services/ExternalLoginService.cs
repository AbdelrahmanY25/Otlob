namespace Otlob.Services;

public class ExternalLoginService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
                                  IUnitOfWorkRepository unitOfWorkRepository) : IExternalLoginService
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public Task<Result<string>> ExternalLoginAsync(string provider, string returnUrl)
    {
        try
        {
            var redirectUrl = $"/Customer/ExternalLogins/ExternalLoginCallback?returnUrl={returnUrl}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            
            return Task.FromResult(Result.Success(redirectUrl));
        }
        catch (Exception)
        {
            return Task.FromResult(Result.Failure<string>(new Error("ExternalLogin.InitiationFailed", "Failed to initiate external login")));
        }
    }

    public async Task<Result<string>> ExternalLoginCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= "/";

        if (remoteError is not null)
            return Result.Failure<string>(new Error("ExternalLogin.RemoteError", $"Error from external provider: {remoteError}"));

        var info = await _signInManager.GetExternalLoginInfoAsync();
        
        if (info is null)
            return Result.Failure<string>(new Error("ExternalLogin.InfoNotFound", "Error loading external login information"));

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        
        if (string.IsNullOrEmpty(email))
            return Result.Failure<string>(new Error("ExternalLogin.EmailNotProvided", "Email was not provided by the external provider"));

        if (result.Succeeded)
        {
            var resultValidUserLogin = await IsValidUser(email!);

            if (resultValidUserLogin.IsFailure)
                return Result.Failure<string>(resultValidUserLogin.Error);

            var logedUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            var role = (await _userManager.GetRolesAsync(logedUser!)).First();

            await _signInManager.SignInWithClaimsAsync(logedUser!, true, await AddUserClaimsAsync(logedUser!, role));

            return Result.Success(role);
        }

        if (result.IsLockedOut)
            return Result.Failure<string>(AuthenticationErrors.LockedOutUser);        

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "",
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                Image = info.Principal.FindFirstValue("picture")
            };

            var createResult = await _userManager.CreateAsync(user);
            
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return Result.Failure<string>(AuthenticationErrors.InvalidRegistration(errors));
            }

            await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        
        if (!addLoginResult.Succeeded)
        {
            var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
            return Result.Failure<string>(AuthenticationErrors.InvalidRegistration(errors));
        }

        var userRole = (await _userManager.GetRolesAsync(user!)).First();
        
        await _signInManager.SignInWithClaimsAsync(user!, true, await AddUserClaimsAsync(user!, userRole));

        return Result.Success(userRole);
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

    private async Task<Result> IsValidUser(string email)
    {
        bool isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == email);

        if (!isEmailExists)
            return Result.Failure(AuthenticationErrors.InvalidCredentials);

        var user = await _userManager.Users
          .AsNoTracking()
          .Where(u => u.Email == email)
          .Select(selector: u => new ApplicationUser
          {
              LockoutEnabled = u.LockoutEnabled,
              LockoutEnd = u.LockoutEnd
          })
          .FirstAsync();

        bool isUserLockoutEnabled = user!.LockoutEnabled;

        if (!isUserLockoutEnabled)
            return Result.Failure(AuthenticationErrors.LockedOutUser);

        var userLockoutEndDate = user.LockoutEnd;

        if (userLockoutEndDate.HasValue && userLockoutEndDate.Value > DateTimeOffset.UtcNow)
            return Result.Failure(AuthenticationErrors.UserLockoutEndDate(userLockoutEndDate.Value.LocalDateTime));

        return Result.Success();
    }
}
