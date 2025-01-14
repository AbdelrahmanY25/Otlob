using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Otlob.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(15)]
        public string? FirstName { get; set; }     

        [MaxLength(15)]
        public string? LastName { get; set; }

        [Required]
        public int Resturant_Id { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }

        [ValidateNever]
        public ICollection<Address> UserAddress { get; set; }

        [ValidateNever]
        public ICollection<Order> Orders { get; set; }

        [ValidateNever]
        public ICollection<UserComplaint> UserComplaints { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }
}
