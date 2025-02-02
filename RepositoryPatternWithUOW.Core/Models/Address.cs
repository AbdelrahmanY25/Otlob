using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Otlob.Core.IServices;

namespace Otlob.Core.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string CustomerAddres { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
