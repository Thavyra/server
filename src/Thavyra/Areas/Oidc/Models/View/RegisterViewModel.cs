using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Thavyra.Oidc.Managers;

namespace Thavyra.Oidc.Models.View;

public partial class RegisterViewModel
{
    public required string ReturnUrl { get; set; }

    [Remote("CheckUsername", "Login", 
        ErrorMessage = "Username already taken.",
        HttpMethod = "post")]
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    
    public partial class Validator : AbstractValidator<RegisterViewModel>
    {
        public Validator(IUserManager users)
        {
            RuleFor(model => model.Username)
                .NotEmpty().WithMessage("Please choose a username.")
                .Length(1, 40).WithMessage("Username must be shorter than 40 characters.")
                .Matches(UsernameRegex()).WithMessage("Invalid special character in username.")
                .MustAsync(users.IsUsernameUniqueAsync).WithMessage("Username already taken.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Please choose a password.")
                .MinimumLength(8).WithMessage("Password must have at least 8 characters.")
                .MaximumLength(100).WithMessage("Password must be shorter than 100 characters.");

            RuleFor(model => model.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }

        [GeneratedRegex(@"([a-zA-Z0-9_\-\'\.]+)")]
        private static partial Regex UsernameRegex();
    }
}