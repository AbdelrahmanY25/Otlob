using Otlob.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Otlob.Core.ViewModel
{
    public class AddressVM
    {
        [Required, MinLength(10), MaxLength(100)]
        public string CustomerAddres { get; set; }

        public static Address MapToAddress(AddressVM addressVM)
        {
            return new Address { CustomerAddres = addressVM.CustomerAddres };
        }

        public static AddressVM MapToAddressVM(Address address)
        {
            return new AddressVM { CustomerAddres = address.CustomerAddres };
        }
    }
}
