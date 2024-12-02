using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.ViewModel
{
    public class RestaurantVM
    {
        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(20, ErrorMessage = "the Length mustn't be greater than 20")]
        public string Name { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(40, ErrorMessage = "the Length mustn't be greater than 40")]
        public string Address { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid EmailAddress")]
        public string Email { get; set; }

        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(100, ErrorMessage = "the Length mustn't be greater than 100")]
        public string Descriptions { get; set; }

        [Required]
        [ValidateNever]
        [NotMapped]
        public IFormFile Logo { get; set; }

        [Range(10, 200, ErrorMessage = "The value must be between 10 and 200.")]
        public decimal DeliveryFee { get; set; }
        public decimal DeliveryDuration { get; set; }
    }
}
