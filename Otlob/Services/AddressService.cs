namespace Otlob.Areas.Customer.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public AddressService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public IQueryable<AddressVM>? GetUserAddressies(string userId)
        {
            return unitOfWorkRepository
                            .Addresses
                            .GetAllWithSelect(selector: add => new AddressVM { AddressVMId = add.Id, CustomerAddres = add.CustomerAddres },
                                             expression: add => add.ApplicationUserId == userId,
                                             tracked: false);
        }

        public AddressVM? GetOneAddress(int addressId)
        {
            return unitOfWorkRepository
                            .Addresses
                            .GetOneWithSelect(selector: add => new AddressVM { CustomerAddres = add.CustomerAddres },
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
                              .Get(expression: add => add.ApplicationUserId == userId && add.CustomerAddres == addressVM.CustomerAddres, tracked: false)
                              .Any();
        }
    }
}
