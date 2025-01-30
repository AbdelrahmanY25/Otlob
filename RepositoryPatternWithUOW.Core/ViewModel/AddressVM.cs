using Otlob.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Otlob.Core.ViewModel
{
    public class AddressVM
    {
        public int Id {  get; set; }
        public string ApplicationUserId { get; set; }

        [Required, MinLength(10), MaxLength(100)]
        public string CustomerAddres { get; set; }

        public static Address MapToAddress(AddressVM addressVM)
        {
            return new Address { ApplicationUserId = addressVM.ApplicationUserId, CustomerAddres = addressVM.CustomerAddres };
        }

        public static AddressVM MapToAddressVM(Address address)
        {
            return new AddressVM { ApplicationUserId = address.ApplicationUserId, CustomerAddres = address.CustomerAddres, Id = address.Id };
        }

    }
}
