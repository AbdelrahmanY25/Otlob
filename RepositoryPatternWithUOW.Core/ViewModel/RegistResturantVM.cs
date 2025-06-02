namespace Otlob.Core.ViewModel
{
    public class RegistResturantVM
    {
        public int Id { get; set; }

        [Length(2, 30), Display(Prompt = "Resturant Name")]
        public string ResName { get; set; } = null!;

        [Length(2, 30), Compare(nameof(ResName)), Display(Prompt = "Resturant UserName")]
        public string ResUserName { get; set; } = null!;

        [Length(10, 100), Display(Prompt = "Resturant Email")]
        [DataType(DataType.EmailAddress)]
        public string ResEmail { get; set; } = null!;

        [Length(3, 100), Display(Prompt = "Resturant Address")]
        public string ResAddress { get; set; } = null!;

        [Display(Prompt = "Resturant Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{5}|\d{11}$", ErrorMessage = "The phone number must contain only numbers and be either 5 or 11 digits.")]
        public string ResPhone { get; set; } = null!;

        [DataType(DataType.Password), Display(Prompt = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Prompt = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "There is no match with Password")]
        public string ConfirmPassword { get; set; } = null!;

        [MinLength(5), Display(Prompt = "Write a Description about your resturant")]
        public string Description { get; set; } = null!;

        public Restaurant MapToRestaurant()
        {
            return new Restaurant
            {
                Name = this.ResName,
                Email = this.ResEmail,
                Address = this.ResAddress.ToString(),
                Phone = this.ResPhone,
                Description = this.Description
            };
        }
    } 
}
