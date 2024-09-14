using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Applications.Patch;

public class Validator : Validator<Request>
{
    public Validator()
    {
        When(x => x.Name.HasValue, () =>
        {
            RuleFor(application => application.Name.Value)
                .NotEmpty()
                .MaximumLength(400).WithMessage("Name must not exceed 400 characters.");
        });
        
        When(x => x.Description.HasValue, () =>
        {
            RuleFor(req => req.Description.Value)
                .NotEmpty()
                .MaximumLength(400).WithMessage("Description must not exceed 400 characters.");
        });
    }
}