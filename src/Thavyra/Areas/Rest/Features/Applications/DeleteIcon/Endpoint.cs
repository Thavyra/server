using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security;
using Thavyra.Storage;

namespace Thavyra.Rest.Features.Applications.DeleteIcon;

public class Endpoint : Endpoint<ApplicationRequest>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IAvatarStorageService _storage;

    public Endpoint(IAuthorizationService authorizationService, IAvatarStorageService storage)
    {
        _authorizationService = authorizationService;
        _storage = storage;
    }

    public override void Configure()
    {
        Delete("/applications/{Application}/icon");
        
        Description(x => x
            .ProducesProblemDetails()
            .ProducesProblemDetails(403));
        
        Summary(x =>
        {
            x.Summary = "Delete Icon";
        });
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().Application is not { } application)
        {
            throw new InvalidOperationException();
        }

        if (await _authorizationService.AuthorizeAsync(User, application, Security.Policies.Operation.Application.Update)
            is { Succeeded: false, Failure: var failure })
        {
            await this.SendAuthorizationFailureAsync(failure, ct);
            return;
        }

        var result = await _storage.DeleteAvatarAsync(AvatarType.Application, application.Id, ct);

        if (result is DeleteFileSucceededResult or DeleteFileNotFoundResult)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendErrorsAsync(cancellation: ct);
    }
}