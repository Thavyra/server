using FastEndpoints;
using FluentValidation;
using OpenIddict.Abstractions;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Applications.Post;

public class Validator : Validator<Request>
{
    public Validator()
    {
        using var scope = CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        
        When(x => x.OwnerId.HasValue, () =>
        {
            RuleFor(req => req.OwnerId.Value)
                .NotEmpty()
                .MustAsync(userService.ExistsAsync);
        });
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(400).WithMessage("Name must not exceed 400 characters.");

        string[] types = [OpenIddictConstants.ApplicationTypes.Web, OpenIddictConstants.ApplicationTypes.Native];
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => types.Contains(x)).WithMessage($"Application type should be one of {string.Join(", ", types)}");

        When(x => x.Description.HasValue, () =>
        {
            RuleFor(req => req.Description.Value)
                .NotEmpty()
                .MaximumLength(400).WithMessage("Description must not exceed 400 characters.");
        });
    }
}