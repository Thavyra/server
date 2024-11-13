using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Thavyra.Oidc.Models.View;

public class LoginViewModel
{
    public required string ReturnUrl { get; set; }
    public string? Message { get; set; }
    
    [Required(ErrorMessage = "Please enter your username!")]
    public string? Username { get; set; }
    
    [Required(ErrorMessage = "Please enter your password!")]
    public string? Password { get; set; }
    
    public class Validator : AbstractValidator<LoginViewModel>
    {
        public Validator()
        {
            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Please enter your username.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Please enter your password.");
        }
    }
}