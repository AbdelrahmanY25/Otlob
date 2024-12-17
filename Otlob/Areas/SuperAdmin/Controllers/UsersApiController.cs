using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Otlob.Core.IUnitOfWorkRepository;
using Otlob.Core.Models;
using System.Linq.Dynamic.Core;

namespace Otlob.Areas.SuperAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersApiController(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.userManager = userManager;
        }

        [HttpPost]
        public IActionResult Index()
        {
            var pageSize = int.Parse(Request.Form["length"]);
            var start = int.Parse(Request.Form["start"]);

            var searchValue = Request.Form["search[value]"];

            var sortColumn = Request.Form[string.Concat("columns[", Request.Form["order[0][column]"], "][name]")];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            var users = unitOfWorkRepository.Users.Get(expression: u => searchValue.IsNullOrEmpty() ? true : 
                                                       u.UserName.Contains(searchValue) || u.Email.Contains(searchValue) ||
                                                       u.PhoneNumber.Contains(searchValue));

             if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
             {
                users = users.AsQueryable().OrderBy(string.Concat(sortColumn, " ", sortColumnDirection)); ;
             }

            var data = users.Skip(start).Take(pageSize);

            var recordsTotal = users.Count();
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data };
            return Ok(jsonData);
        }             
    }
}
