namespace Otlob.Core.ViewModel
{
    public class AddressVM
    {
        public string? Key { get; set; }

        [Length(10, 100), RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Address must start with a letter")]
        public string CustomerAddress { get; set; } = null!;

        public Address MapToAddress(string userId, int addresId = 0)
        {
            Address address;

            if (addresId == 0)
                return address = new Address { ApplicationUserId = userId, CustomerAddres = this.CustomerAddress! };

            return new Address { Id = addresId, CustomerAddres = this.CustomerAddress!, ApplicationUserId = userId };
        } 
    }
}
