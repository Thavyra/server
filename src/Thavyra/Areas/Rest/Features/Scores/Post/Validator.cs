using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Scores.Post;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ObjectiveId)
            .NotEmpty();
    }
}