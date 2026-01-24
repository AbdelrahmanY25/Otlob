using Google.Apis.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace Otlob.ApiServices;

public class ApiAuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                            IJwtProvider jwtProvider, ISendEmailsToUsersService sendEmailsToUsersService, IUnitOfWorkRepository unitOfWorkRepository) : IApiAuthService
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly ISendEmailsToUsersService _sendEmailsToUsersService = sendEmailsToUsersService;
    
    private readonly int _refreshTokenExpiryDayes = 14;

    public async Task<ApiResult> RegisterAsync(MobileRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var isExists = await IsUserEmailExists(request, cancellationToken);

        if (isExists.IsFailure)
            return ApiResult.Failure(isExists.ApiError);

        ApplicationUser user = new() { UserName = request.Email, Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };

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



    public async Task<ApiResult> ConfirmEmailViaOtpAsync(OtpRequest request)
    {
        bool isOtpValid = _unitOfWorkRepository.Otps.IsExist(o => o.Code == request.Otp && !o.IsUsed && o.CreatedAt.AddSeconds(90) >= DateTime.UtcNow);

        if (!isOtpValid)
            return ApiResult.Failure(UserErrors.InvalidOrExpiredOtp);

        var otp = _unitOfWorkRepository.Otps.GetOne(expression: o => o.Code == request.Otp && !o.IsUsed);

        otp!.IsUsed = true;        

        _unitOfWorkRepository.Otps.Update(otp);            
        _unitOfWorkRepository.SaveChanges();

        await _userManager.Users
            .Where(u => u.Id == otp.UserId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.EmailConfirmed, true));

        return ApiResult.Success();
    }

    public async Task<bool> IsUserExistsAsync(ResendEmailConfirmationRequest request)
    {
        bool isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email);

        return isEmailExists;
    }

    public async Task<ApiResult<AuthResponse>> GoogleSignInAsync(GoogleIdTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Validate the Google ID token
        GoogleJsonWebSignature.Payload payload;
        
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch (Exception)
        {
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.InvalidGoogleIdToken);
        }

        // Ensure email is verified by Google
        if (!payload.EmailVerified)
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.GoogleEmailNotVerified);

        var email = payload.Email;

        if (string.IsNullOrEmpty(email))
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.EmailNotProvided);

        // Check if user exists with this external login
        var user = await _userManager.FindByLoginAsync("Google", payload.Subject);

        if (user is not null)
        {
            // User already has Google login linked - return tokens
            var resultValidUserLogin = await IsValidUser(email, cancellationToken);

            if (resultValidUserLogin.IsFailure)
                return ApiResult.Failure<AuthResponse>(resultValidUserLogin.ApiError);

            bool isUserHasActiveRefreshToken = user.RefreshTokens.Any(r => r.IsActive);

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

        // Check if user exists with this email
        user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            // Create new user
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = payload.GivenName ?? "",
                LastName = payload.FamilyName ?? "",
                Image = payload.Picture
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                var error = createResult.Errors.First();
                return ApiResult.Failure<AuthResponse>(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
        }

        // Add the Google login to the user
        var loginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
        var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!addLoginResult.Succeeded)
        {
            var error = addLoginResult.Errors.First();
            return ApiResult.Failure<AuthResponse>(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        // Generate and return tokens
        return await GenerateTokenAndRefreshTokenAsync(user);
    }

    public async Task<ApiResult<AuthResponse>> MicrosoftSignInAsync(MicrosoftIdTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Validate and decode the Microsoft ID token
        var handler = new JwtSecurityTokenHandler();
        
        if (!handler.CanReadToken(request.IdToken))
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.InvalidMicrosoftIdToken);

        JwtSecurityToken jwtToken;
        
        try
        {
            jwtToken = handler.ReadJwtToken(request.IdToken);
        }
        catch (Exception)
        {
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.InvalidMicrosoftIdToken);
        }

        // Verify the token is not expired
        if (jwtToken.ValidTo < DateTime.UtcNow)
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.InvalidMicrosoftIdToken);

        // Extract claims from the token
        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == "preferred_username")?.Value;
        var subject = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "oid")?.Value;
        var givenName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
        var familyName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
        var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        if (string.IsNullOrEmpty(email))
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.EmailNotProvided);

        if (string.IsNullOrEmpty(subject))
            return ApiResult.Failure<AuthResponse>(ExternalSignInErrors.InvalidMicrosoftIdToken);

        // Check if user exists with this external login
        var user = await _userManager.FindByLoginAsync("Microsoft", subject);

        if (user is not null)
        {
            // User already has Microsoft login linked - return tokens
            var resultValidUserLogin = await IsValidUser(email, cancellationToken);

            if (resultValidUserLogin.IsFailure)
                return ApiResult.Failure<AuthResponse>(resultValidUserLogin.ApiError);

            bool isUserHasActiveRefreshToken = user.RefreshTokens.Any(r => r.IsActive);

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

        // Check if user exists with this email
        user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            // Parse name if first/last names not provided
            string firstName = givenName ?? "";
            string lastName = familyName ?? "";
            
            if (string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(name))
            {
                var nameParts = name.Split(' ', 2);
                firstName = nameParts[0];
                lastName = nameParts.Length > 1 ? nameParts[1] : "";
            }

            // Create new user
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                var error = createResult.Errors.First();
                return ApiResult.Failure<AuthResponse>(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
        }

        // Add the Microsoft login to the user
        var loginInfo = new UserLoginInfo("Microsoft", subject, "Microsoft");
        var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!addLoginResult.Succeeded)
        {
            var error = addLoginResult.Errors.First();
            return ApiResult.Failure<AuthResponse>(new(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        // Generate and return tokens
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

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        _unitOfWorkRepository.Otps.Add(new Otp { Code = otp, UserId = user.Id, CreatedAt = DateTime.UtcNow });

        _unitOfWorkRepository.SaveChanges();

        BackgroundJob.Enqueue(() => _sendEmailsToUsersService.ConfirmEmailViaOtpAsync(otp, user));

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
