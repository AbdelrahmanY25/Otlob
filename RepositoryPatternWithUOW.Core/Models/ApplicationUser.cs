using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Utility;

namespace Otlob.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int RestaurantId { get; set; }
        public byte[]? Image { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }

        [ValidateNever]
        public  Restaurant Restaurant { get; set; }

        [ValidateNever]
        public  ICollection<Address> UserAddress { get; set; }

        [ValidateNever]
        public  ICollection<Order> Orders { get; set; }

        [ValidateNever]
        public  ICollection<UserComplaint> UserComplaints { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }
}
