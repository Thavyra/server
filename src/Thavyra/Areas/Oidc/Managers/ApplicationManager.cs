using MassTransit;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Oidc.Models;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Managers;

public class ApplicationManager : OpenIddictApplicationManager<ApplicationModel>
{
    private readonly IRequestClient<Application_CheckClientSecret> _client;

    public ApplicationManager(
        IRequestClient<Application_CheckClientSecret> client,
        IOpenIddictApplicationCache<ApplicationModel> cache,
        ILogger<ApplicationManager> logger,
        IOptionsMonitor<OpenIddictCoreOptions> options,
        IOpenIddictApplicationStoreResolver resolver)
        : base(cache, logger, options, resolver)
    {
        _client = client;
    }

    public override async ValueTask<bool> ValidateClientSecretAsync(ApplicationModel application, string secret,
        CancellationToken cancellationToken = new())
    {
        Response response = await _client.GetResponse<Correct, Incorrect>(new Application_CheckClientSecret
        {
            ApplicationId = application.Id,
            Secret = secret
        }, cancellationToken);

        return response switch
        {
            (_, Correct) => true,
            (_, Incorrect) => false,
            _ => throw new InvalidOperationException()
        };
    }
}