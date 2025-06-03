namespace Otlob.Core.ViewModel
{
    public class ProfileVM : ImageUrl
    {
        public string? ProfileVMId { get; set; }

        [Required, MaxLength(100), Display(Prompt = "Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [MaxLength(15), Display(Prompt = "FirstName")]
        [RegularExpression(@"^[a-zA-Z]{1,15}$", ErrorMessage = "The FirstName must be only letters.")]
        public string? FirstName { get; set; }

        [MaxLength(15), Display(Prompt = "LastName")]
        [RegularExpression(@"^[a-zA-Z]{1,15}$", ErrorMessage = "The LastName must be only letters.")]
        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        [Display(Prompt = "Date of Birth")]
        public DateOnly? BirthDate { get; set; }

        [Display(Prompt = "Phone Number")]
        [Required, DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{11}$", 
            ErrorMessage = "The phone number must contain only numbers and be up to 11 digits long.")]
        public string? PhoneNumber { get; set; }

        public byte[]? Image { get; set; }
    }
}
