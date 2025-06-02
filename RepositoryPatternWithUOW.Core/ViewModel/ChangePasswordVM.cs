namespace Otlob.Core.ViewModel
{
    public class ChangePasswordVM
    {
        public int Id { get; set; }

        [Required, Display(Prompt = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

        [Required, Display(Prompt = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required, Display(Prompt = "Confirm New Password")]
        [DataType(DataType.Password), Compare("NewPassword", ErrorMessage = "There is no match with new password")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
