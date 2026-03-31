using Microsoft.AspNetCore.Mvc;

namespace TasSteeltoeSample.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Content(
            "TAS Steeltoe sample is running. Hit this route a few times, then open Apps Manager → Steeltoe → Trace.");
    }
}
