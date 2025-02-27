using Microsoft.AspNetCore.Identity;
using Otlob.Core.IServices;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;
using Utility;

namespace Otlob.Core.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;
        public RegisterService(IUnitOfWorkRepository.IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }

        public async Task<bool> RegisterRestaurant(RegistResturantVM registresturant)
        {
            var applicationUser = new ApplicationUser { UserName = registresturant.ResUserName, Email = registresturant.ResEmail };

            var result = await userManager.CreateAsync(applicationUser, registresturant.Password);

            if (!result.Succeeded)
            {
                return false;
            }

            await userManager.AddToRoleAsync(applicationUser, SD.restaurantAdmin);

            var restaurant = registresturant.MapToRestaurant();

            unitOfWorkRepository.Restaurants.Create(restaurant);
            unitOfWorkRepository.SaveChanges();

            applicationUser.RestaurantId = restaurant.Id;
            await userManager.UpdateAsync(applicationUser);

            var userAddress = new Address { CustomerAddres = registresturant.ResAddress, ApplicationUserId = applicationUser.Id };

            unitOfWorkRepository.Addresses.Create(userAddress);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public async Task<bool> RegisterSuperAdmin(ApplicationUserlVM userVM)
        {
            var applicatioUser = new ApplicationUser { UserName = userVM.UserName, Email = userVM.Email, PhoneNumber = userVM.PhoneNumber };

            var result = await userManager.CreateAsync(applicatioUser, userVM.Password);

            if (!result.Succeeded)
            {
                return false;
            }

            await userManager.AddToRoleAsync(applicatioUser, SD.superAdminRole);

            var userAddress = new Address { CustomerAddres = userVM.Address, ApplicationUserId = applicatioUser.Id, };

            unitOfWorkRepository.Addresses.Create(userAddress);
            unitOfWorkRepository.SaveChanges();

            return true;
        }
    }
}
