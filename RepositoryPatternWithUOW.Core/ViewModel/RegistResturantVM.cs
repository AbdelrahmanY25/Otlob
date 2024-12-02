using Otlob.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [RegularExpression(@"^\d{5,11}$", ErrorMessage = "The phone number must contain only numbers and be up to 11 digits long.")]
        public string ResPhone { get; set; } = null!;

        [Required, DataType(DataType.Password), Display(Prompt = "Password")]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password), Display(Prompt = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "There is no match with Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required,MinLength(10), Display(Prompt = "Write a Description about your resturant")]
        public string Description { get; set; } = null!;
    } 
}
