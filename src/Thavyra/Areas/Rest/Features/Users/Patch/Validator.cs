using System.Text.RegularExpressions;
using FluentValidation;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.User;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Patch;

public partial class Validator : AbstractValidator<Request>
{
    public Validator(IUserService userService)
    {
        When(x => x.Username.HasValue, () =>
        {
            RuleFor(user => user.Username.Value)
                .NotEmpty()
                .Length(1, 40)
                .Matches(UsernameRegex()).WithMessage("Invalid special character.")
                .MustAsync(userService.IsUsernameUniqueAsync).WithMessage("Username already taken.");
        });

        When(x => x.Description.HasValue, () =>
        {
            RuleFor(user => user.Description.Value)
                .Length(1, 400);
        });
    }

    [GeneratedRegex(@"([a-zA-Z0-9_-]+)")]
    private static partial Regex UsernameRegex();
}