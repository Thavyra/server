using FastEndpoints;
using FluentValidation;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Features.Objectives.Post;

public class Validator : Validator<Request>
{
    private readonly IRequestClient<Objective_ExistsByName> _exists;

    public Validator(IRequestClient<Objective_ExistsByName> exists)
    {
        _exists = exists;
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(40)
            .Must(x => x.All(c => char.IsLetterOrDigit(c) || c == '.')).WithMessage("Name may only contain alphanumeric characters and '.'")
            .Must(x => !x.StartsWith('.')).WithMessage("Name may not start with '.'")
            .Must(x => !x.EndsWith('.')).WithMessage("Name may not end with '.'")
            .MustAsync(ExistsAsync).WithMessage("Objective with the same name already exists.");
        
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(40);
    }

    private async Task<bool> ExistsAsync(Request request, string name, CancellationToken cancellationToken)
    {
        Response response = await _exists.GetResponse<ObjectiveExists, NotFound>(new Objective_ExistsByName
        {
            ApplicationId = request.ApplicationId,
            Name = name
        }, cancellationToken);

        return response switch
        {
            (_, NotFound) => false,
            (_, ObjectiveExists) => true,
            _ => throw new InvalidOperationException()
        };
    }
}