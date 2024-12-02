using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class Point
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public DateTime ExpireDate { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }
    }
}
