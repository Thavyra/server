using FastEndpoints;
using FluentValidation;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Features.Objectives.Post;

public class Validator : Validator<Request>
{
    private readonly IRequestClient<Objective_ExistsByName> _exists;

    public Validator()
    {
        using var scope = CreateScope();
        _exists = scope.ServiceProvider.GetRequiredService<IRequestClient<Objective_ExistsByName>>();
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(40)
            .Must(x => x.All(c => char.IsLetterOrDigit(c) || c == '.')).WithMessage("Name may only contain alphanumeric characters and '.'")
            .Must(x => !x.StartsWith('.')).WithMessage("Name may not start with '.'")
            .Must(x => !x.EndsWith('.')).WithMessage("Name may not end with '.'")
            .MustAsync(NotExistAsync).WithMessage("Objective with the same name already exists.");
        
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(40);
    }

    private async Task<bool> NotExistAsync(Request request, string name, CancellationToken cancellationToken)
    {
        Response response = await _exists.GetResponse<ObjectiveExists, NotFound>(new Objective_ExistsByName
        {
            ApplicationId = request.ApplicationId,
            Name = name
        }, cancellationToken);

        return response switch
        {
            (_, NotFound) => true,
            (_, ObjectiveExists) => false,
            _ => throw new InvalidOperationException()
        };
    }
}