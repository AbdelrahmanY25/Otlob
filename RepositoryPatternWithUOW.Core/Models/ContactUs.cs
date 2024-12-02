using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class ContactUs
    {
        public int Id {  get; set; }
        public string ResName { get; set; }
        public string ResUserName { get; set; }
        public string ResEmail { get; set; }
        public string ResAddress { get; set; }
        public string ResPhone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Description { get; set; }
    }
}
