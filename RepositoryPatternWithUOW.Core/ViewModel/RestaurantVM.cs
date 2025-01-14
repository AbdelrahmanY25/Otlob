using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Otlob.Core.ViewModel
{
    public class RestaurantVM
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(20, ErrorMessage = "the Length mustn't be greater than 20")]
        public string Name { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(40, ErrorMessage = "the Length mustn't be greater than 40")]
        public string Address { get; set; }

        [Required, Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{5}|\d{11}$", ErrorMessage = "The phone number must contain only numbers and be either 5 or 11 digits.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid EmailAddress")]
        [Required, MaxLength(100), Display(Prompt = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        public string Description { get; set; }
        public decimal DeliveryDuration { get; set; }

        [Range(0, 200, ErrorMessage = "The value must be between 0 and 200.")]
        public decimal DeliveryFee { get; set; }
       
        [ValidateNever]
        [NotMapped]
        public string Logo { get; set; }

        [ValidateNever]
        public AcctiveStatus AcctiveStatus { get; set; }

        public static RestaurantVM MapToRestaurantVM(Restaurant restaurant)
        {
            return new RestaurantVM
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                Email = restaurant.Email,
                Description = restaurant.Description,
                DeliveryDuration = restaurant.DeliveryDuration,
                DeliveryFee = restaurant.DeliveryFee,
                AcctiveStatus = restaurant.AcctiveStatus,
                Logo = restaurant.Logo
            };
        }

        public static Restaurant MapToRestaurant(RestaurantVM restaurantVM, Restaurant oldRestaurant)
        {
           oldRestaurant.Name = restaurantVM.Name;
           oldRestaurant.Address = restaurantVM.Address;
           oldRestaurant.Phone = restaurantVM.Phone;
           oldRestaurant.Email = restaurantVM.Email;
           oldRestaurant.Description = restaurantVM.Description;
           oldRestaurant.DeliveryDuration = restaurantVM.DeliveryDuration;
           oldRestaurant.DeliveryFee = restaurantVM.DeliveryFee;
           oldRestaurant.Logo = restaurantVM.Logo;

            return oldRestaurant;
        }
    }
}
