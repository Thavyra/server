using System.Security.Claims;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.Internal;
using Thavyra.Oidc.Models.View;

namespace Thavyra.Oidc.Controllers;

[Area("Oidc")]
public class LoginController : Controller
{
    private readonly IUserManager _userManager;

    public LoginController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    [FromQuery] public string ReturnUrl { get; set; } = "/";

    [HttpGet("/login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        return View(new LoginViewModel
        {
            ReturnUrl = ReturnUrl
        });
    }

    [HttpPost("/login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(
        LoginViewModel model,
        [FromServices] IValidator<LoginViewModel> validator,
        CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        var validation = await validator.ValidateAsync(model, cancellationToken);

        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return View(new LoginViewModel
            {
                ReturnUrl = ReturnUrl
            });
        }

        ArgumentException.ThrowIfNullOrEmpty(model.Username);
        ArgumentException.ThrowIfNullOrEmpty(model.Password);

        var user = await _userManager.FindByLoginAsync(new PasswordLoginModel
        {
            Username = model.Username,
            Password = model.Password
        }, cancellationToken);

        if (user is null)
        {
            const string message = "Username or password incorrect.";

            ModelState.AddModelError(nameof(model.Username), message);
            ModelState.AddModelError(nameof(model.Password), message);

            return View(new LoginViewModel
            {
                ReturnUrl = ReturnUrl
            });
        }

        return SignIn(user.Id);
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        return View(new RegisterViewModel
        {
            ReturnUrl = ReturnUrl
        });
    }

    [HttpPost("/register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAsync(
        RegisterViewModel model,
        [FromServices] IValidator<RegisterViewModel> validator,
        CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        var validation = await validator.ValidateAsync(model, cancellationToken);

        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return View(new RegisterViewModel
            {
                ReturnUrl = ReturnUrl
            });
        }

        ArgumentException.ThrowIfNullOrEmpty(model.Username);
        ArgumentException.ThrowIfNullOrEmpty(model.Password);

        var user = await _userManager.RegisterAsync(new PasswordRegisterModel
        {
            Username = model.Username,
            Password = model.Password
        }, cancellationToken);

        return SignIn(user.Id);
    }

    [HttpPost("/register/check-username")]
    public async Task<IActionResult> CheckUsernameAsync(RegisterViewModel model, CancellationToken cancellationToken)
    {
        bool valid = model.Username is not null
                     && await _userManager.IsUsernameUniqueAsync(model.Username, cancellationToken);

        return new JsonResult(valid);
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private SignInResult SignIn(Guid userId)
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6),
            IsPersistent = true
        };
        
        return SignIn(new ClaimsPrincipal(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}