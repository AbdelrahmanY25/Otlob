using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.Models
{
    public class UserComplaint
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int RestaurantId { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Check;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        public Restaurant Restaurant { get; set; }
    }
    public enum RequestStatus
    {
        Check,
        Invalid,
        Resolved
    }
}
