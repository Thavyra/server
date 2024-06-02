using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Oidc.Models;

namespace Thavyra.Oidc.Stores;

public class ApplicationStore : BaseApplicationStore
{
    private readonly IScopedClientFactory _clientFactory;

    public ApplicationStore(IScopedClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    private static ApplicationModel Map(Application application)
    {
        return new ApplicationModel
        {
            Id = application.Id,
            ApplicationType = application.Type,
            ClientType = application.ClientType,
            ClientId = application.ClientId,
            ConsentType = application.ConsentType,
            DisplayName = application.Name
        };
    }
    
    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Application_Count>();

        var response = await client.GetResponse<Value<long>>(new Application_Count(), cancellationToken);

        return response.Message.Item;
    }

    public override async IAsyncEnumerable<ApplicationModel> ListAsync(int? count, int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Application_List>();

        var response = await client.GetResponse<Multiple<Application>>(new Application_List
        {
            Count = count,
            Offset = offset
        }, cancellationToken);

        foreach (var application in response.Message.Items)
        {
            yield return Map(application);
        }
    }

    public override async ValueTask<ApplicationModel?> FindByIdAsync(string identifier,
        CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Application_GetById>();

        Response response = await client.GetResponse<Application, NotFound>(new Application_GetById
        {
            Id = identifier
        }, cancellationToken);

        return response switch
        {
            (_, Application application) => Map(application),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async ValueTask<ApplicationModel?> FindByClientIdAsync(string identifier,
        CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Application_GetByClientId>();

        Response response = await client.GetResponse<Application, NotFound>(new Application_GetByClientId
        {
            ClientId = identifier
        }, cancellationToken);

        return response switch
        {
            (_, Application application) => Map(application),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async IAsyncEnumerable<ApplicationModel> FindByRedirectUriAsync(string uri,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Application_GetByRedirect>();

        var response = await client.GetResponse<Multiple<Application>>(new Application_GetByRedirect
        {
            Uri = uri
        }, cancellationToken);

        foreach (var application in response.Message.Items)
        {
            yield return Map(application);
        }
    }

    public override async ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(ApplicationModel application,
        CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Redirect_GetByApplication>();

        var response = await client.GetResponse<Multiple<Redirect>>(new Redirect_GetByApplication
        {
            ApplicationId = application.Id
        }, cancellationToken);

        return response.Message.Items
            .Select(x => x.Uri)
            .ToImmutableArray();
    }

    public override ValueTask<ApplicationModel> InstantiateAsync(CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public override ValueTask CreateAsync(ApplicationModel application, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public override ValueTask DeleteAsync(ApplicationModel application, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public override ValueTask UpdateAsync(ApplicationModel application, CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public override IAsyncEnumerable<ApplicationModel> FindByPostLogoutRedirectUriAsync(string uri,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}