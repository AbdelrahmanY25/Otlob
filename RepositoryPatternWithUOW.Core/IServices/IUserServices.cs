using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Core.IServices
{
    public interface IUserServices
    {
        ApplicationUser ValidateUserInfo(ApplicationUser user, ProfileVM profileVM);
    }
}
