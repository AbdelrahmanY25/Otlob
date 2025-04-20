using Otlob.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Otlob.Core.ViewModel
{
    public class CustomerConcernVM
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        [Required]
        public int ResturantId { get; set; }

        [Required]
        public string Description { get; set; } = null!;
        public RequestStatus Status { get; set; } = RequestStatus.Check;
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
