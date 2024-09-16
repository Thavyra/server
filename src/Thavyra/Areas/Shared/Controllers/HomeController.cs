using System.Diagnostics;
using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Thavyra.Contracts.User;
using Thavyra.Shared.Models;

namespace Thavyra.Shared.Controllers;

[Area("Shared")]
public class HomeController : Controller
{
    private readonly IRequestClient<User_GetById> _client;

    public HomeController(IRequestClient<User_GetById> client)
    {
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated is not true)
        {
            return Challenge();
        }
        
        var response = await _client.GetResponse<User>(new User_GetById
        {
            Id = Guid.Parse(User.GetClaim(ClaimTypes.NameIdentifier)!)
        });

        return View(new UserViewModel
        {
            User = response.Message
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