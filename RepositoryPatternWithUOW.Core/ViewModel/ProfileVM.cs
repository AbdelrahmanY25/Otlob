namespace Otlob.Core.ViewModel
{
    public class ProfileVM
    {
        public string? ProfileVMId { get; set; }

        [Display(Prompt = "Email"), DataType(DataType.EmailAddress), Length(10, 50, ErrorMessage = "Email length between 10 to 50 letters")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z]+\.(com)$",
        ErrorMessage = "Email must start with a letter and end with .com")]
        public string Email { get; set; } = null!;

        [MaxLength(15), Display(Prompt = "First Name")]
        [RegularExpression(@"^[a-zA-Z]{1,15}$", ErrorMessage = "The FirstName must be only letters.")]
        public string? FirstName { get; set; }

        [MaxLength(15), Display(Prompt = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]{1,15}$", ErrorMessage = "The LastName must be only letters.")]
        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        [Display(Prompt = "Date of Birth"), DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        [Display(Prompt = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{11}$", 
            ErrorMessage = "The phone number must contain only numbers and be up to 11 digits long.")]
        public string PhoneNumber { get; set; } = null!;

        [DataType(DataType.Upload)]
        public string? Image { get; set; }
    }
}
