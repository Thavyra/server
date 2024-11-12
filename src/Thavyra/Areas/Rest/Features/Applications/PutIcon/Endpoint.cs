using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security;
using Thavyra.Storage;

namespace Thavyra.Rest.Features.Applications.PutIcon;

public class Endpoint : Endpoint<Request>
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
        Put("/applications/{Application}/icon");
        AllowFileUploads();
        
        Summary(x =>
        {
            x.Summary = "Upload Icon";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
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

        var result = await _storage.UploadAvatarAsync(AvatarType.Application, application.Id, req.Icon.OpenReadStream(), ct);

        if (result is not UploadFileSucceededResult)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        await SendCreatedAtAsync<GetIcon.Endpoint>(new {Application = application.Id}, null, cancellation: ct);
    }
}