using System.Diagnostics.CodeAnalysis;

namespace Otlob.Core.ViewModel
{
    public class RestaurantVM
    {
        public int RestaurantVMId { get; set; }
        public string? Key { get; set; }
        public string? UserId { get; set; }

        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(20, ErrorMessage = "the Length mustn't be greater than 20")]
        public string Name { get; set; } = null!;

        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(40, ErrorMessage = "the Length mustn't be greater than 40")]
        public string Address { get; set; } = null!;

        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{5}|\d{11}$", ErrorMessage = "The phone number must contain only numbers and be either 5 or 11 digits.")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Please enter a valid EmailAddress")]
        [MaxLength(100), Display(Prompt = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        public string Description { get; set; } = null!;
        public decimal DeliveryDuration { get; set; }

        [Range(0, 200, ErrorMessage = "The value must be between 0 and 200.")]
        public decimal DeliveryFee { get; set; }

        [Range(0, 3)]
        public AcctiveStatus AcctiveStatus { get; set; } = AcctiveStatus.Unaccepted;

        [Range(0, 15)]
        public RestaurantCategory Category { get; set; }

        [AllowNull, ValidateNever]
        public string Image { get; set; }
    }
}
