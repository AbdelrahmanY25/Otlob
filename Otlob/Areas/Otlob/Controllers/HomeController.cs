namespace Otlob.Areas.Otlob.Controllers;

[Area(SD.otlob)]
public class HomeController : Controller
{
    public IActionResult Home() => View("HomePage");
}
