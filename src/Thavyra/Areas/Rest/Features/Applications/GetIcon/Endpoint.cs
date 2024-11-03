using FastEndpoints;
using Thavyra.Rest.Services;

namespace Thavyra.Rest.Features.Applications.GetIcon;

public class Endpoint : Endpoint<Request>
{
    private readonly IIconService _iconService;

    public Endpoint(IIconService iconService)
    {
        _iconService = iconService;
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
        
        var image = await _iconService.GetDefaultIconAsync(
            seed: application.Name,
            size: req.Size ?? 128,
            cancellationToken: ct);

        await SendStreamAsync(image, fileName: "icon.png", contentType: "image/png", cancellation: ct);
    }
}