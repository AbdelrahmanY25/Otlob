namespace Otlob.Core.ViewModel
{
    public class ChangePasswordVM
    {
        [Display(Prompt = "Old Password"), DataType(DataType.Password),
            MinLength(6, ErrorMessage = "The Password musty be mixture of letters, numbers and special sympols with min length 6 letter")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Password must start with a letter")]
        public string OldPassword { get; set; } = null!;

        [Display(Prompt = "New Password"), DataType(DataType.Password),
            MinLength(6, ErrorMessage = "The Password musty be mixture of letters, numbers and special sympols with min length 6 letter")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Password must start with a letter")]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "There is no match with new password")]
        [Display(Prompt = "Confirm New Password"), DataType(DataType.Password),
            MinLength(6, ErrorMessage = "The Password musty be mixture of letters, numbers and special sympols with min length 6 letter")]
        [RegularExpression(@"^[a-zA-Z].*", ErrorMessage = "Password must start with a letter")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
