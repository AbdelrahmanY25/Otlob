using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.ViewModel
{
    public class LoginVM
    {
        public int Id { get; set; }

        [Required, Display(Prompt = "Email")]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Prompt = "Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
