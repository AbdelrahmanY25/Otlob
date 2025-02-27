using Otlob.Areas.Customer.Services.Interfaces;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Otlob.Core.ViewModel;

namespace Otlob.Areas.Customer.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public AddressService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public bool AddUserAddress(string customerAddres, string userId)
        {
            var userAddress = new Address { ApplicationUserId = userId, CustomerAddres = customerAddres };

            unitOfWorkRepository.Addresses.Create(userAddress);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public bool CheckOnAddressIfExist(string userId, AddressVM addressVM)
        {
           return unitOfWorkRepository
                              .Addresses
                              .Get(expression: add => add.ApplicationUserId == userId && add.CustomerAddres == addressVM.CustomerAddres, tracked: false)
                              .Any();
        }
    }
}
