namespace Otlob.Core.ViewModel
{
    public class AddressVM
    {
        [ValidateNever]
        public int AddressVMId { get; set; }

        [Required, MinLength(10), MaxLength(100)]
        public string CustomerAddres { get; set; }

        public Address MapToAddress(string userId, int addresId = 0)
        {
            Address address;

            if (addresId == 0)
                return address = new Address { ApplicationUserId = userId, CustomerAddres = this.CustomerAddres };

            return new Address { Id = addresId, CustomerAddres = this.CustomerAddres, ApplicationUserId = userId };
        } 
    }
}
