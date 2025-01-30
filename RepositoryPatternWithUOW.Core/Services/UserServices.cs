using Otlob.Core.IServices;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Core.Services
{
    public class UserServices : IUserServices
    {        
        public ApplicationUser ValidateUserInfo(ApplicationUser user, ProfileVM profileVM)
        {
            user.Email = profileVM.Email;
            user.FirstName = profileVM.FirstName;
            user.LastName = profileVM.LastName;
            user.Gender = profileVM.Gender;
            user.PhoneNumber = profileVM.PhoneNumber;
            user.BirthDate = profileVM.BirthDate;
            return user;
        }          
    }
}
