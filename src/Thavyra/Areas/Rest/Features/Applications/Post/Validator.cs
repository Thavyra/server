using FluentValidation;
using OpenIddict.Abstractions;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Applications.Post;

public class Validator : AbstractValidator<Request>
{
    public Validator(IUserService userService)
    {
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
            .Must(x => types.Contains(x)).WithMessage("Unknown application type.");

        When(x => x.ConsentType.HasValue, () =>
        {
            string[] consentTypes =
                [OpenIddictConstants.ConsentTypes.Explicit, OpenIddictConstants.ConsentTypes.Implicit];
            RuleFor(req => req.ConsentType.Value)
                .NotEmpty()
                .Must(y => consentTypes.Contains(y)).WithMessage("Unknown consent type.");
        });

        When(x => x.Description.HasValue, () =>
        {
            RuleFor(req => req.Description.Value)
                .NotEmpty()
                .MaximumLength(400).WithMessage("Description must not exceed 400 characters.");
        });
    }
}