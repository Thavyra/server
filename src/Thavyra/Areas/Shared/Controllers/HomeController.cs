using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Thavyra.Shared.Models;

namespace Thavyra.Shared.Controllers;

[Area("Shared")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated is not true)
        {
            return Challenge();
        }

        return View(new UserViewModel
        {
            Id = User.GetClaim(ClaimTypes.NameIdentifier),
            Username = User.GetClaim(ClaimTypes.Name)
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}