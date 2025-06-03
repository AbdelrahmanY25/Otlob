using Otlob.Core.Attributes;

namespace Otlob.Core.ViewModel
{
    public class LoginVM
    {
        [ValidEmail]
        [Display(Prompt = "Email"), DataType(DataType.EmailAddress), Length(10, 50, ErrorMessage = "Email length between 10 to 50 letters")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z]+\.(com)$",
        ErrorMessage = "Email must start with a letter and end with .com")]
        public string Email { get; set; }

        [Display(Prompt = "Password"), DataType(DataType.Password),
            MinLength(6, ErrorMessage = "The Password musty be mixture of letters, numbers and special sympols with min length 6 letter")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Password must start with a letter")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
