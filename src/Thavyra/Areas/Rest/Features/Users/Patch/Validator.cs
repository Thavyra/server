using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Users.Patch;

public partial class Validator : Validator<Request>
{
    public Validator()
    {
        using var scope = CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        
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