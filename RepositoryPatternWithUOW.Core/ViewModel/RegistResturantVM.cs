using RepositoryPatternWithUOW.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Otlob.Core.ViewModel
{
    public class RegistResturantVM
    {
        public int Id { get; set; }

        [MaxLength(50), Display(Prompt = "Resturant Name")]
        public string ResName { get; set; } = null!;

        [Required, MaxLength(50), Compare(nameof(ResName)), Display(Prompt = "Resturant UserName")]
        public string ResUserName { get; set; } = null!;

        [Required, MaxLength(100), Display(Prompt = "Resturant Email")]
        [DataType(DataType.EmailAddress)]
        public string ResEmail { get; set; } = null!;

        [Required, MaxLength(100), Display(Prompt = "Resturant Address")]
        public string ResAddress { get; set; } = null!;

        [Required, Display(Prompt = "Resturant Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{5}|\d{11}$", ErrorMessage = "The phone number must contain only numbers and be either 5 or 11 digits.")]
        public string ResPhone { get; set; } = null!;

        [Required, DataType(DataType.Password), Display(Prompt = "Password")]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password), Display(Prompt = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "There is no match with Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required,MinLength(10), Display(Prompt = "Write a Description about your resturant")]
        public string Description { get; set; } = null!;

        public static Restaurant MapToRestaurant(RegistResturantVM registresturant)
        {
            return new Restaurant
            {
                Name = registresturant.ResName,
                Email = registresturant.ResEmail,
                Address = registresturant.ResAddress.ToString(),
                Phone = registresturant.ResPhone,
                Description = registresturant.Description
            };
        }
    } 
}
