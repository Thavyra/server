using FluentValidation;

namespace Thavyra.Rest.Features.Transactions.Post;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Amount)
            .NotEqual(0);
        
        When(x => x.Description.HasValue, () =>
        {
            RuleFor(x => x.Description.Value)
                .Length(2, 40);
        });
    }
}