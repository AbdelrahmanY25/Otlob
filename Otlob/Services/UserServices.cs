﻿namespace Otlob.Core.Services
{
    public class UserServices : IUserServices
    {
        private readonly IDataProtector dataProtector;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;

        public UserServices(UserManager<ApplicationUser> userManager,
                            IDataProtectionProvider dataProtectionProvider,
                            IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository)
        {
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }

        public IQueryable<ApplicationUser>? GetAllUsers(Expression<Func<ApplicationUser, bool>>? query = null)
        {
            var users = unitOfWorkRepository
                        .Users
                        .GetAllWithSelect(
                            expression: query,
                            tracked: false,
                            selector: u => new ApplicationUser
                            {
                                Id = dataProtector.Protect(u.Id),
                                UserName = u.UserName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                LockoutEnabled = u.LockoutEnabled,
                            }
                        );

            return users!.OrderBy(u => u.UserName);
        }

        public void ChangeBlockUserStatus(string userId, bool blockstatus)
        {
            ApplicationUser user = userManager.FindByIdAsync(dataProtector.Unprotect(userId)).GetAwaiter().GetResult()!;

            if (user is not null)
            {
                user.LockoutEnabled = blockstatus;
                unitOfWorkRepository.Users.ModifyProperty(user, u => u.LockoutEnabled);
                unitOfWorkRepository.SaveChanges();
            }
        }

        public ApplicationUser UpdateUserInfo(ApplicationUser user, ProfileVM profileVM)
        {
            user.Email = profileVM.Email;
            user.FirstName = profileVM.FirstName;
            user.LastName = profileVM.LastName;
            user.Gender = profileVM.Gender;
            user.PhoneNumber = profileVM.PhoneNumber;
            user.BirthDate = profileVM.BirthDate;

            return user;
        }

        public ApplicationUser UpdateUserProfile(ProfileVM profileVM, string userId)
        {
            ApplicationUser? user = unitOfWorkRepository.Users.GetOne(expression: u => u.Id == userId)!;

            return UpdateUserInfo(user, profileVM);
        }

        public ProfileVM GetUserProfileVmDetails(string userId)
        {
            ProfileVM? userProfile = unitOfWorkRepository.Users
               .GetOneWithSelect
                (
                    expression: u => u.Id == userId,
                    tracked: false,
                    selector: u => new ProfileVM
                    {
                        Email = u.Email!,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Image = u.Image,
                        BirthDate = u.BirthDate,
                        PhoneNumber = u.PhoneNumber!,
                        Gender = u.Gender,
                    }
                );

            return userProfile!;
        }

        public ApplicationUser? GetUserDataToPartialview(string userId)
        {
            ApplicationUser? user = unitOfWorkRepository.Users.
                GetOneWithSelect(
                    expression: u => u.Id == userId,
                    tracked: false,
                    selector: u => new ApplicationUser
                    {
                        UserName = u.UserName,
                        Image = u.Image,
                        PhoneNumber = u.PhoneNumber,
                        Email = u.Email
                    }
                );

            return user;
        }

        public int GetUserRestaurantId(string userId)
        {
            ApplicationUser? user = unitOfWorkRepository.Users
                .GetOneWithSelect(
                    expression: u => u.Id == userId,
                    tracked: false,
                    selector: u => new ApplicationUser
                    {
                        RestaurantId = u.RestaurantId
                    }
                );

            if (user is null)
            {
                return 0;
            }

            return user.RestaurantId;
        }

        public string? GetUserIdByRestaurantId(int restaurantId)
        {
            ApplicationUser? user = unitOfWorkRepository.Users
                .GetOneWithSelect(
                    expression: u => u.RestaurantId == restaurantId,
                    tracked: false,
                    selector: u => new ApplicationUser
                    {
                        Id = u.Id
                    }
                );

            return user?.Id;
        }
                
        public void UpdateUserImage(ApplicationUser user, string image)
        {
            user.Image = image;
            unitOfWorkRepository.Users.ModifyProperty(user, u => u.Image!);
            unitOfWorkRepository.SaveChanges();
        }
    }
}
