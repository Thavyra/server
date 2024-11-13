using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Thavyra.Oidc.Managers;

namespace Thavyra.Oidc.Models.View;

public partial class RegisterViewModel
{
    public required string ReturnUrl { get; set; }
    public string? Message { get; set; }

    [Required(ErrorMessage = "Required")]
    [Length(1, 40, ErrorMessage = "Too long!")]
    [RegularExpression(@"^[a-zA-Z0-9_\-\'\.]+$", ErrorMessage = "Invalid character!")]
    [Remote("CheckUsername", "Login",
        ErrorMessage = "Username not available!",
        HttpMethod = "post")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Required")]
    [MinLength(8, ErrorMessage = "Must have at least 8 characters!")]
    [MaxLength(50, ErrorMessage = "Too long!")]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "Required")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
    public string? ConfirmPassword { get; set; }

    public partial class Validator : AbstractValidator<RegisterViewModel>
    {
        public Validator(IUserManager users)
        {
            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Please choose a username.")
                .NotEqual("me").WithMessage("Username not available!")
                .Length(1, 40).WithMessage("Username must be shorter than 40 characters.")
                .Matches(UsernameRegex()).WithMessage("Invalid special character in username.")
                .MustAsync(users.IsUsernameUniqueAsync).WithMessage(x => $"{x.Username} is not available!");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Please choose a password.")
                .MinimumLength(8).WithMessage("Password must have at least 8 characters.")
                .MaximumLength(100).WithMessage("Password must be shorter than 100 characters.");

            RuleFor(model => model.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }

        [GeneratedRegex(@"^[a-zA-Z0-9_\-\'\.]+$")]
        private static partial Regex UsernameRegex();
    }
}