using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Utility;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Area("SuperAdmin"), Authorize(Roles = SD.superAdminRole)]
    public class ComplaintController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public ComplaintController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult UserComplaints()
        {
            var UserComplaints = unitOfWorkRepository.UserComplaints.Get([c => c.User, c => c.Restaurant]);
            return View(UserComplaints);
        }
    }
}
