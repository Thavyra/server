using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Scoreboard.Post;

public class Validator : Validator<Request>
{
    public Validator()
    {
        When(x => x.ObjectiveId.HasValue, () =>
        {
            RuleFor(x => x.ObjectiveId.Value)
                .NotEmpty();

            RuleFor(x => x.ObjectiveName)
                .Empty().WithMessage("Provide a value for only one of objective id or name.");
        });

        When(x => x.ObjectiveName.HasValue, () =>
        {
            RuleFor(x => x.ObjectiveName.Value)
                .NotEmpty();

            RuleFor(x => x.ObjectiveId)
                .Empty().WithMessage("Provide a value for only one of objective id or name.");
        });
    }
}