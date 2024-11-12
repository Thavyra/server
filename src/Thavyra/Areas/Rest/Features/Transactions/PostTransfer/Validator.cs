using FastEndpoints;
using FluentValidation;

namespace Thavyra.Rest.Features.Transactions.PostTransfer;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RecipientId)
            .NotEmpty()
            .NotEqual(x => x.Subject);
        
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfers may only deduct from the current user.");
        
        When(x => x.Description.HasValue, () =>
        {
            RuleFor(x => x.Description.Value)
                .Length(2, 40);
        });
    }
}