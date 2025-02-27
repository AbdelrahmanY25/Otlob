using Otlob.Core.Models;
using System.ComponentModel.DataAnnotations;


namespace Otlob.Core.ViewModel
{
    public class OrderVM
    {
        public int Id { get; set; }
     
        [Required]
        public IQueryable<Address> Addresses { get; set; }

        [Required]
        public int ResturantId { get; set; }

        [Required]
        public int CartId { get; set; }
       
        [Required]
        public PaymentMethod Method { get; set; }

        public string? Notes { get; set; }
    }
}
