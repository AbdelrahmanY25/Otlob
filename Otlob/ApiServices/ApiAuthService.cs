namespace Otlob.ApiServices;

public class ApiAuthService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
                            ISendEmailsToUsersService sendEmailsToUsersService) : IApiAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ISendEmailsToUsersService _sendEmailsToUsersService = sendEmailsToUsersService;

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
}
