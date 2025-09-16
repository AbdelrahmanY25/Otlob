namespace Otlob.Services;

public class UserServices(UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider,
                          IUnitOfWorkRepository unitOfWorkRepository) : IUserServices
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("SecureData");
    private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

    public IQueryable<ApplicationUser>? GetAllUsers(Expression<Func<ApplicationUser, bool>>? query = null)
    {
        var users = _unitOfWorkRepository
                    .Users
                    .GetAllWithSelect(
                        expression: query,
                        tracked: false,
                        selector: u => new ApplicationUser
                        {
                            Id = _dataProtector.Protect(u.Id),
                            UserName = u.UserName,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            LockoutEnabled = u.LockoutEnabled,
                        }
                    );

        return users!.OrderBy(u => u.UserName);
    }

    public int GetCustomersCount()
    {
        return _unitOfWorkRepository.Users.Get(tracked: false)!.Count();
    }

    public async Task<Result> ToggleUserBlockStatusAsync(string userId)
    {
        if (userId is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        bool isUserIdExist = await _userManager.Users.AnyAsync(u => u.Id == _dataProtector.Unprotect(userId));

        if (!isUserIdExist)
        {
            return Result.Failure(AuthenticationErrors.InvalidUser);
        }

        ApplicationUser user = (await _userManager.FindByIdAsync(_dataProtector.Unprotect(userId)))!;  

        user.LockoutEnabled = !user.LockoutEnabled;

        _unitOfWorkRepository.Users.ModifyProperty(user, u => u.LockoutEnabled);
        _unitOfWorkRepository.SaveChanges();

        return Result.Success();
    }   

    public async Task<Result<ApplicationUser>> GetUserContactInfo(string userId)
    {
        if (userId is null)
        {
            return Result.Failure<ApplicationUser>(AuthenticationErrors.InvalidUser);
        }

        bool isUserIdExist = await _userManager.Users.AnyAsync(u => u.Id == userId);

        if (!isUserIdExist)
        {
            return Result.Failure<ApplicationUser>(AuthenticationErrors.InvalidUser);
        }

        ApplicationUser user = _unitOfWorkRepository.Users.
            GetOneWithSelect(
                expression: u => u.Id == userId,
                tracked: false,
                selector: u => new ApplicationUser
                {
                    Email = u.Email,
                    Image = u.Image,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber
                }
            )!;

        return Result.Success(user);
    }       
}
