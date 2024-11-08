using FastEndpoints;
using Thavyra.Rest.Services;
using Thavyra.Storage;

namespace Thavyra.Rest.Features.Applications.GetIcon;

public class Endpoint : Endpoint<Request>
{
    private readonly IIconService _iconService;
    private readonly IAvatarStorageService _storageService;

    public Endpoint(IIconService iconService, IAvatarStorageService storageService)
    {
        _iconService = iconService;
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("/applications/{Application}/icon.png");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (ProcessorState<AuthenticationState>().Application is not { } application)
        {
            throw new InvalidOperationException();
        }

        var result = await _storageService.GetAvatarAsync(AvatarType.Application, application.Id, ct);

        var image = result switch
        {
            GetFileSucceededResult file => file.Stream,
            _ => await _iconService.GetDefaultIconAsync(
                seed: application.Name,
                size: req.Size ?? 500,
                cancellationToken: ct)
        };

        await SendStreamAsync(image, fileName: "icon.png", contentType: "image/png", cancellation: ct);
    }
}