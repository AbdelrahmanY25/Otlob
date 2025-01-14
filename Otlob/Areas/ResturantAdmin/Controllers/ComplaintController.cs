using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using Utility;

namespace Otlob.Areas.ResturantAdmin.Controllers
{
    [Area("ResturantAdmin"), Authorize(Roles = SD.restaurantAdmin)]
    public class ComplaintController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ComplaintController(IUnitOfWorkRepository unitOfWorkRepository,
                                   UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public  async Task<IActionResult> CustomerComplaints()
        {
            var user = await userManager.GetUserAsync(User);
            var UserComplaints = unitOfWorkRepository.UserComplaints.Get([c => c.User, c => c.Restaurant], c => c.RestaurantId == user.Resturant_Id);
            return View(UserComplaints);
        }
    }
}
