namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class UsersAnalysisController(IUsersAnalysisService usersAnalysisService) : Controller
{       
    private readonly IUsersAnalysisService _usersAnalysisService = usersAnalysisService;

    public IActionResult Index()
    {            
        UsersAnalysisVM usersAnalysisVM = _usersAnalysisService.GetCusomersAndPartnersCount();

        return View(usersAnalysisVM);
    }
}
