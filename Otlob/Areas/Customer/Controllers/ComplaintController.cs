using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;

namespace Otlob.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ComplaintController : Controller
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ComplaintController(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }
        public IActionResult AddComplaint()
        {
            var userResturants = unitOfWorkRepository.Carts.Get(expression: r => r.ApplicationUserId == userManager.GetUserId(User));

            var resturants = new List<Restaurant>();

            foreach (var res in userResturants)
            {
                var resturnat = unitOfWorkRepository.Restaurants.GetOne(expression: r => r.Id == res.RestaurantId);
                if (resturnat != null) 
                    resturants.Add(resturnat);  
            }

            ViewBag.userRes = resturants;
            return View();
        }

        [HttpPost]
        public IActionResult AddComplaint(UserComplaint complaint)
        {
            var userComplaint = unitOfWorkRepository.UserComplaints.GetOne(expression: c => c.UserId == complaint.UserId && c.RestaurantId == complaint.RestaurantId);
            if (userComplaint != null)
            {
                TempData["Error"] = "You Can't Complaint Again Please Wait Your Complaint Under Processing From Resturant And our Company";
            }
            else
            {
                unitOfWorkRepository.UserComplaints.Create(complaint);
                unitOfWorkRepository.SaveChanges();

                TempData["Success"] = "Your Complaints Uploaded";
            }            

            return RedirectToAction("Index", "Home");
        }
    }
}
