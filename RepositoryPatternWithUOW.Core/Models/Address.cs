namespace Otlob.Core.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = null!;
        public string CustomerAddres { get; set; } = null!;

        [ValidateNever]
        public ApplicationUser User { get; set; } = null!;
    }
}
