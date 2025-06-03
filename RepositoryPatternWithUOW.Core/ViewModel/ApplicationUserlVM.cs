namespace Otlob.Core.ViewModel
{
    public class ApplicationUserlVM
    {
        [Display(Prompt = "User Name"), DataType(DataType.Text)]
        [RegularExpression(@"^(?!.*@.+\.com$)[a-zA-Z].*", ErrorMessage = "User Name must start with a letter and not end with @...com")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Email length must be between 10 and 20 characters")]
        public string UserName { get; set; }

        [Display(Prompt = "Email"), DataType(DataType.EmailAddress), Length(10, 50, ErrorMessage = "Email length between 10 to 50 letters")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z]+\.(com)$",
        ErrorMessage = "Email must start with a letter and end with .com")]
        public string Email { get; set; }

        [Display(Prompt = "Password"), DataType(DataType.Password),
            MinLength(6, ErrorMessage = "The Password musty be mixture of letters, numbers and special sympols with min length 6 letter")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Password must start with a letter")]
        public string Password { get; set; }

        [Display(Prompt = "Password"), DataType(DataType.Password),
            MinLength(6, ErrorMessage = "The Password musty be mixture of letters, numbers and special sympols with min length 6 letter")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Confirm Password must start with a letter")]
        [Compare(nameof(Password), ErrorMessage = "There is no match with Password")]
        public string ConfirmPassword { get; set; }

        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Address must start with a letter")]
        public string Address { get; set; }

        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "The phone number must contain only numbers and be up to 11 digits long.")]
        public string PhoneNumber { get; set; }
    }
}
