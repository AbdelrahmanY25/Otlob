﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Otlob.Core.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string CustomerAddres { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
