using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Redirects.Post;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Uri)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _)).WithMessage("Invalid URI.");
    }
}