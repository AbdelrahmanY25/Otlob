using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }

        [Required, MinLength(10)]
        public string CustomerAddres { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
