﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Otlob.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(15)]
        public string? FirstName { get; set; }     

        [MaxLength(15)]
        public string? LastName { get; set; }

        [Required]
        public int Resturant_Id { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }

        [ValidateNever]
        public ICollection<Address> UserAddress { get; set; }
    }
}