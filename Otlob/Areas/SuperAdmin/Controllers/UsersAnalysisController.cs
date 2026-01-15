namespace Otlob.Areas.SuperAdmin.Controllers;

[Area(DefaultRoles.SuperAdmin), Authorize(Roles = DefaultRoles.SuperAdmin)]
public class UsersAnalysisController(IUsersAnalysisService usersAnalysisService) : Controller
{       
    private readonly IUsersAnalysisService _usersAnalysisService = usersAnalysisService;

    public async Task<IActionResult> Index()
    {            
        UsersAnalysisResponse usersAnalysisResponse = await _usersAnalysisService.GetCusomersCount();

        return View(usersAnalysisResponse);
    }
}
