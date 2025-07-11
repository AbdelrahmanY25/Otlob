using Microsoft.AspNetCore.DataProtection;

namespace Otlob.Areas.Customer.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IDataProtector dataProtector;


        public AddressService(IUnitOfWorkRepository unitOfWorkRepository, IDataProtectionProvider dataProtectionProvider)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            dataProtector = dataProtectionProvider.CreateProtector("SecureData");
        }

        public IQueryable<AddressVM>? GetUserAddressies(string userId)
        {
            return unitOfWorkRepository
                            .Addresses
                            .GetAllWithSelect(selector: add => new AddressVM
                            { 
                                Key = dataProtector.Protect(add.Id.ToString()),
                                CustomerAddress = add.CustomerAddres
                            },
                            expression: add => add.ApplicationUserId == userId,
                            tracked: false);
        }

        public AddressVM? GetOneAddress(int addressId)
        {
            return unitOfWorkRepository
                            .Addresses
                            .GetOneWithSelect(selector: add => new AddressVM { CustomerAddress = add.CustomerAddres },
                                              expression: a => a.Id == addressId,
                                              tracked: false);
        }

        public string? AddAddress(AddressVM addressVM, string userId)
        {
            if(IsAddressExist(userId, addressVM))
            {
                return "The address is already exist";
            }

            var address = addressVM.MapToAddress(userId);

            unitOfWorkRepository.Addresses.Create(address);
            unitOfWorkRepository.SaveChanges();

            return null;
        }

        public bool AddUserAddress(string customerAddres, string userId)
        {
            var userAddress = new Address { ApplicationUserId = userId, CustomerAddres = customerAddres };

            unitOfWorkRepository.Addresses.Create(userAddress);
            unitOfWorkRepository.SaveChanges();

            return true;
        }

        public string? UpdateAddress(AddressVM addressVM, string userId, int addressId)
        {
            Address address = addressVM.MapToAddress(userId, addressId);
            unitOfWorkRepository.Addresses.Edit(address);
            unitOfWorkRepository.SaveChanges();
            return null;
        }

        public bool DeleteAddress(int addressId)
        {
            unitOfWorkRepository.Addresses.SoftDelete(expression: add => add.Id == addressId);
            unitOfWorkRepository.SaveChanges();
            return true;
        }

        public bool IsAddressExist(string userId, AddressVM addressVM)
        {           
            return unitOfWorkRepository
                .Addresses
                .IsExist(expression: add => add.ApplicationUserId == userId && add.CustomerAddres == addressVM.CustomerAddress);
        }

        public bool IsUserHasAnyAddress(string userId)
        {           
            return unitOfWorkRepository
                .Addresses
                .IsExist(expression: add => add.ApplicationUserId == userId);
        }


    }
}
