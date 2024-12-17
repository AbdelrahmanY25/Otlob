using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.ViewModel
{
    public class MealVm
    {
        public int Id { get; set; }

        [Required ,MinLength(3)]
        public string Name { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; }       

        [Required]
        public MealCategory Category { get; set; }
    }
}
