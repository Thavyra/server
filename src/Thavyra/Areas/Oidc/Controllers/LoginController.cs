using System.Security.Claims;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.Login.Password;
using Thavyra.Oidc.Managers;
using Thavyra.Oidc.Models.View;

namespace Thavyra.Oidc.Controllers;

[Area("Oidc"), Route("/accounts")]
public class LoginController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IRequestClient<PasswordLogin> _login;
    private readonly IRequestClient<RegisterUser> _register;

    public LoginController(IUserManager userManager,
        IRequestClient<PasswordLogin> login,
        IRequestClient<RegisterUser> register)
    {
        _userManager = userManager;
        _login = login;
        _register = register;
    }

    private string? _returnUrl;
    [FromQuery]
    public string ReturnUrl
    {
        get => Url.IsLocalUrl(_returnUrl) ? _returnUrl : "/";
        set => _returnUrl = value;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View(new LoginViewModel
        {
            ReturnUrl = ReturnUrl
        });
    }

    [HttpPost("login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(
        LoginViewModel model,
        [FromServices] IValidator<LoginViewModel> validator,
        CancellationToken cancellationToken)
    {
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

        try
        {
            Response response = await _login.GetResponse<LoginSucceeded, LoginFailed, LoginNotFound>(new PasswordLogin
            {
                Username = model.Username,
                Password = model.Password
            }, cancellationToken);

            switch (response)
            {
                case (_, LoginFailed or LoginNotFound):
                    const string message = "Username or password incorrect.";

                    ModelState.AddModelError(nameof(model.Username), message);
                    ModelState.AddModelError(nameof(model.Password), message);

                    return View(model);
                
                case (_, LoginSucceeded login):
                    return SignIn(login.UserId, login.Username);
            }
        }
        catch (RequestFaultException)
        {
            model.Message = "Uh oh! Something went wrong while logging in. Please try again.";
        }

        return View(model);
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(new RegisterViewModel
        {
            ReturnUrl = ReturnUrl
        });
    }

    [HttpPost("register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAsync(
        RegisterViewModel model,
        [FromServices] IValidator<RegisterViewModel> validator,
        CancellationToken cancellationToken)
    {
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

        try
        {
            var response = await _register.GetResponse<UserRegistered>(new RegisterUser
            {
                Username = model.Username,
                Password = model.Password
            }, cancellationToken);

            return SignIn(response.Message.UserId, response.Message.Username);
        }
        catch (RequestFaultException)
        {
            model.Message = "Uh oh! Something went wrong during registration. Please try again.";
        }
        
        return View(model);
    }

    [HttpPost("register/check-username")]
    public async Task<IActionResult> CheckUsernameAsync(RegisterViewModel model, CancellationToken cancellationToken)
    {
        bool valid = model.Username is not null
                     && await _userManager.IsUsernameUniqueAsync(model.Username, cancellationToken);

        return new JsonResult(valid);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = ReturnUrl
        }, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private SignInResult SignIn(Guid userId, string username)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            RedirectUri = ReturnUrl,
        };
        
        return SignIn(new ClaimsPrincipal(identity), properties, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}