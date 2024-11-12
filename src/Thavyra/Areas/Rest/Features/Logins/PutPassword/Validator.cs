using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Logins.PutPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}