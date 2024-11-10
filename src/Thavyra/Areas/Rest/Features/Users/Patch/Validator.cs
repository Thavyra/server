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
                .NotEqual("me").WithMessage("Not available.")
                .Matches(@"^[a-zA-Z0-9_\-\'\.]+$").WithMessage("Contains invalid character.")
                .MustAsync(userService.IsUsernameUniqueAsync).WithMessage("Not available.");
        });

        When(x => x.Description.HasValue, () =>
        {
            RuleFor(user => user.Description.Value)
                .Length(1, 400);
        });
    }
}