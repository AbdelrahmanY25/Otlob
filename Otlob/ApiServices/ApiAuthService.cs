using System.Security.Cryptography;

namespace Otlob.ApiServices;

public class ApiAuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                            IJwtProvider jwtProvider, IHttpContextAccessor httpContextAccessor, 
                            ISendEmailsToUsersService sendEmailsToUsersService) : IApiAuthService
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ISendEmailsToUsersService _sendEmailsToUsersService = sendEmailsToUsersService;

    private readonly int _refreshTokenExpiryDayes = 14;

    public async Task<ApiResult> RegisterAsync(MobileRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var isExists = await IsUserEmailExists(request, cancellationToken);

        if (isExists.IsFailure)
            return ApiResult.Failure(isExists.ApiError);

        ApplicationUser user = new() { UserName = request.Email, Email = request.Email };

        var creationResult = await _userManager.CreateAsync(user, request.Password);

        if (!creationResult.Succeeded)
        {
            var error = creationResult.Errors.First();

            return ApiResult.Failure(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        await SendEmailConfirmationAsync(user);

        await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);

        return ApiResult.Success();
    }

    public async Task<ApiResult> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return ApiResult.Failure(UserErrors.UserNotFound);

        return await SendEmailConfirmationAsync(user);
    }

    public async Task<ApiResult<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var isValidUser = await IsValidUser(request.Email, cancellationToken);

        if (isValidUser.IsFailure)
            return ApiResult.Failure<AuthResponse>(isValidUser.ApiError);

        var user = await _userManager.FindByEmailAsync(request.Email);

        var result = await _signInManager.PasswordSignInAsync(user!, request.Password, false, true);

        if (!result.Succeeded)
            return ApiResult.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);

        bool isUserHasActiveRefreshToken = user!.RefreshTokens.Any(r => r.IsActive);

        if (isUserHasActiveRefreshToken)
        {
            RefreshToken refreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive)!;

            var userRole = (await _userManager.GetRolesAsync(user)).First();

            var (token, expiresOn) = _jwtProvider.GenerateToken(user, userRole);

            return ApiResult.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, user.PhoneNumber,
                                                           token, expiresOn, refreshToken.Token, refreshToken.ExpiresOn));
        }

        return await GenerateTokenAndRefreshTokenAsync(user);
    }

    public async Task<ApiResult<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken)
    {
        var result = _jwtProvider.ValidateToken(token)!;

        if (result.IsFailure)
            return ApiResult.Failure<AuthResponse>(result.ApiError);

        string userId = result.Value!;

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResult.Failure<AuthResponse>(UserErrors.UserNotFound);

        RefreshToken userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive)!;

        if (userRefreshToken is null)
            return ApiResult.Failure<AuthResponse>(TokenErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        return await GenerateTokenAndRefreshTokenAsync(user);
    }

    public async Task<ApiResult<bool>> RevokeRefreshTokenAsync(string token, string refreshToken)
    {
        var result = _jwtProvider.ValidateToken(token)!;

        if (result.IsFailure)
            return ApiResult.Failure<bool>(result.ApiError);

        string userId = result.Value!;

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResult.Failure<bool>(UserErrors.UserNotFound);

        RefreshToken? userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);

        if (userRefreshToken is null)
            return ApiResult.Failure<bool>(TokenErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return ApiResult.Success(true);
    }

    public async Task<ApiResult<AuthResponse>> ExternalLoginCallbackAsync(string? returnUrl = null, string? remoteError = null, CancellationToken cancellationToken = default)
    {
        returnUrl ??= "/";

        if (remoteError is not null)
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.RemoteError);

        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info is null)
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.InfoNotFound);

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.EmailNotProvided);

        if (result.Succeeded)
        {
            var resultValidUserLogin = await IsValidUser(email!, cancellationToken);

            if (resultValidUserLogin.IsFailure)
                return ApiResult.Failure<AuthResponse>(resultValidUserLogin.ApiError);

            var logedUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            var role = (await _userManager.GetRolesAsync(logedUser!)).First();

            bool isUserHasActiveRefreshToken = logedUser!.RefreshTokens.Any(r => r.IsActive);

            if (isUserHasActiveRefreshToken)
            {
                RefreshToken refreshToken = logedUser.RefreshTokens.FirstOrDefault(r => r.IsActive)!;

                var (token, expiresOn) = _jwtProvider.GenerateToken(logedUser, role);

                return ApiResult.Success(new AuthResponse(logedUser.Id, logedUser.Email, logedUser.FirstName, logedUser.LastName, logedUser.PhoneNumber,
                                                               token, expiresOn, refreshToken.Token, refreshToken.ExpiresOn));
            }

            return await GenerateTokenAndRefreshTokenAsync(logedUser);
        }

        if (result.IsLockedOut)
            return ApiResult.Failure<AuthResponse>(UserErrors.LockedOutUser);

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
                var error = createResult.Errors.First();
                return ApiResult.Failure<AuthResponse>(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);

        if (!addLoginResult.Succeeded)
        {
            var error = addLoginResult.Errors.First();
            return ApiResult.Failure<AuthResponse>(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var userRole = (await _userManager.GetRolesAsync(user!)).First();

        bool hasActiveRefreshToken = user!.RefreshTokens.Any(r => r.IsActive);

        if (hasActiveRefreshToken)
        {
            RefreshToken refreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive)!;

            var (token, expiresOn) = _jwtProvider.GenerateToken(user, userRole);

            return ApiResult.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, user.PhoneNumber,
                                                           token, expiresOn, refreshToken.Token, refreshToken.ExpiresOn));
        }

        return await GenerateTokenAndRefreshTokenAsync(user);
    }



    private async Task<ApiResult<AuthResponse>> GenerateTokenAndRefreshTokenAsync(ApplicationUser user)
    {
        var userRole = (await _userManager.GetRolesAsync(user)).First();

        var (token, expiresOn) = _jwtProvider.GenerateToken(user, userRole);

        string refreshToken = GenerateRefreshToken();

        DateTime refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDayes);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration,
        });

        await _userManager.UpdateAsync(user);

        return ApiResult.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, user.PhoneNumber,
                                                       token, expiresOn, refreshToken, refreshTokenExpiration));
    }

    private async Task<ApiResult> SendEmailConfirmationAsync(ApplicationUser user)
    {
        if (user.EmailConfirmed)
            return ApiResult.Failure(UserErrors.DoublicatedConfirmation);

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var callbackUrl = $"{origin}/Customer/Account/ConfirmEmail?userId={user.Id}&token={token}";

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.ConfirmEmailAsync(callbackUrl, user));

        return ApiResult.Success();
    }

    private async Task<ApiResult> IsUserEmailExists(MobileRegisterRequest request, CancellationToken cancellationToken)
    {
        bool isEmailExist = await _userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (isEmailExist)
            return ApiResult.Failure(UserErrors.DoublicatedEmail);

        return ApiResult.Success();
    }

    private async Task<ApiResult> IsValidUser(string email, CancellationToken cancellationToken)
    {
        bool isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);

        if (!isEmailExists)
            return ApiResult.Failure(UserErrors.InvalidCredentials);

        var user = await _userManager.Users
          .AsNoTracking()
          .Where(u => u.Email == email)
          .Select(selector: u => new ApplicationUser
          {
              LockoutEnabled = u.LockoutEnabled,
              LockoutEnd = u.LockoutEnd
          })
          .FirstOrDefaultAsync(cancellationToken);

        bool isUserLockoutEnabled = user!.LockoutEnabled;

        if (!isUserLockoutEnabled)
            return ApiResult.Failure(UserErrors.LockedOutUser);

        var userLockoutEndDate = user.LockoutEnd;

        if (userLockoutEndDate.HasValue && userLockoutEndDate.Value > DateTimeOffset.UtcNow)
            return ApiResult.Failure(UserErrors.UserLockoutEndDate(userLockoutEndDate.Value.LocalDateTime));

        return ApiResult.Success();
    }
    
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
