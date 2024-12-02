using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class CustomerConcern
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Order Order { get; set; }
        public int OrderId {get; set; }
        public enum RequestStatus
        {
            Check,
            Invalid,
            Resolved
        }
        public RequestStatus Status { get; set; } = RequestStatus.Check;
        public DateTime DateTime { get; set; }
    }
}
