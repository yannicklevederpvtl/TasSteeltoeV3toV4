using Microsoft.AspNetCore.Mvc;

namespace TasSteeltoeSampleV4.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Content(
            "Steeltoe 4.x TAS sample. Refresh a few times, then Apps Manager → Steeltoe → Trace. " +
            "Set OTEL_EXPORTER_ZIPKIN_ENDPOINT to export spans to Zipkin.");
    }
}
