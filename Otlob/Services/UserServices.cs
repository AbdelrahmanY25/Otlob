using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Core.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;

        public UserServices(IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
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

        public ApplicationUser UpdateRestaurantAdminInfo(ProfileVM profileVM, int restaurantId)
        {
            ApplicationUser? user = unitOfWorkRepository.Users.GetOne(expression: u => u.RestaurantId == restaurantId);

            return UpdateUserInfo(user, profileVM);
        }

        public ProfileVM ViewUserProfileVmDetails(string? userId = null, int? restaurantId = null)
        {
            ProfileVM? userProfile = unitOfWorkRepository.Users
               .GetOneWithSelect
                (
                    expression: u => u.Id == userId || u.RestaurantId == restaurantId,
                    tracked: false,
                    selector: u => new ProfileVM
                    {
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Image = u.Image,
                        BirthDate = u.BirthDate,
                        PhoneNumber = u.PhoneNumber,
                        Gender = u.Gender,
                    }
                );

            return userProfile;
        }
    }
}
