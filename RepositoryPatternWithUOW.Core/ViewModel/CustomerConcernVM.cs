using Otlob.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
