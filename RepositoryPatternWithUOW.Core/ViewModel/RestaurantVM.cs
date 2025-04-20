using Otlob.Core.Models;
using System.ComponentModel.DataAnnotations;
using Utility;

namespace Otlob.Core.ViewModel
{
    public class RestaurantVM : ImageUrl
    {
        public int RestaurantVMId { get; set; }

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

        [Range(0, 3)]
        public AcctiveStatus AcctiveStatus { get; set; } = AcctiveStatus.Unaccepted;

        [Range(0, 15), Required]
        public RestaurantCategory Category { get; set; }
        public byte[]? Image { get; set; }
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
                Image = restaurant.Image
            };
        }
        public static RestaurantVM MapToRestaurantVMWithId(Restaurant restaurant)
        {
            RestaurantVM restaurantVM = MapToRestaurantVM(restaurant);
            restaurantVM.RestaurantVMId = restaurant.Id;

            return restaurantVM;
        }

        public Restaurant MapToRestaurant(Restaurant oldRestaurant)
        {
           oldRestaurant.Name = this.Name;
           oldRestaurant.Address = this.Address;
           oldRestaurant.Phone = this.Phone;
           oldRestaurant.Email = this.Email;
           oldRestaurant.Description = this.Description;
           oldRestaurant.DeliveryDuration = this.DeliveryDuration;
           oldRestaurant.DeliveryFee = this.DeliveryFee;

            return oldRestaurant;
        }
    }
}
