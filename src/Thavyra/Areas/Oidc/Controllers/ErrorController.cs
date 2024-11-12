using Microsoft.AspNetCore.Mvc;

namespace Thavyra.Oidc.Controllers;

public class ErrorController : Controller
{
    [Route("/accounts/error")]
    public IActionResult Index()
    {
        return View();
    }
}