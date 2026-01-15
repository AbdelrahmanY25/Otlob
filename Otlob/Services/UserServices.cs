namespace Otlob.Services;

public class UserServices(UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider,
                        IUnitOfWorkRepository unitOfWorkRepository) : IUserServices
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");

    public async Task<IOrderedEnumerable<UserMainResponse>?> GetAllCustomers()
    {
        var customers = (await _userManager.GetUsersInRoleAsync(DefaultRoles.Customer))
            .Select(
                c => new UserMainResponse
                {
                    Id = _dataProtector.Protect(c.Id),
                    UserName = c.UserName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Image = c.Image,
                    LockoutEnabled = c.LockoutEnabled,
                    EmailConfirmed = c.EmailConfirmed
                }
            );

        if (customers is null)
            return null;

        return customers.OrderBy(u => u.UserName);
    }

    public async Task<Result> ToggleUserBlockStatusAsync(string userId)
    {
        if (userId is null)
            return Result.Failure(AuthenticationErrors.InvalidUser);

        bool isUserIdExist = await _userManager.Users
            .AnyAsync(u => u.Id == _dataProtector.Unprotect(userId));

        if (!isUserIdExist)
            return Result.Failure(AuthenticationErrors.InvalidUser);

        ApplicationUser user = (await _userManager.FindByIdAsync(_dataProtector.Unprotect(userId)))!;  

        user.LockoutEnabled = !user.LockoutEnabled;

        _unitOfWorkRepository.Users.ModifyProperty(user, u => u.LockoutEnabled);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }

    public async Task<Result> ToggleConfirmEmailAsync(string userId)
    {
        if (userId is null)
            return Result.Failure(AuthenticationErrors.InvalidUser);

        bool isUserIdExist = await _userManager.Users
            .AnyAsync(u => u.Id == _dataProtector.Unprotect(userId));

        if (!isUserIdExist)
            return Result.Failure(AuthenticationErrors.InvalidUser);

        ApplicationUser user = (await _userManager.FindByIdAsync(_dataProtector.Unprotect(userId)))!;  

        user.EmailConfirmed = !user.EmailConfirmed;

        _unitOfWorkRepository.Users.ModifyProperty(user, u => u.EmailConfirmed);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }    
}
