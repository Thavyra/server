using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Objectives.Post;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(40);
    }
}