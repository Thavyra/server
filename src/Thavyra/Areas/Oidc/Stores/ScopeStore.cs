using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Scope;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Stores;

public class ScopeStore : BaseScopeStore
{
    private readonly IClientFactory _clientFactory;

    public ScopeStore(IClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    private static ScopeModel Map(Scope scope)
    {
        return new ScopeModel
        {
            Id = scope.Id,
            Name = scope.Name,
            DisplayName = scope.DisplayName,
            Description = scope.Description
        };
    }

    public override async IAsyncEnumerable<ScopeModel> ListAsync(int? count, int? offset,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Scope_List>();

        var response = await client.GetResponse<Multiple<Scope>>(new Scope_List
        {
            Count = count,
            Offset = offset
        }, cancellationToken);

        foreach (var scope in response.Message.Items)
        {
            yield return Map(scope);
        }
    }

    public override async ValueTask<ScopeModel?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Scope_GetById>();

        Response response = await client.GetResponse<Scope, NotFound>(new Scope_GetById
        {
            Id = identifier
        }, cancellationToken);

        return response switch
        {
            (_, Scope scope) => Map(scope),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async ValueTask<ScopeModel?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Scope_GetByName>();

        Response response = await client.GetResponse<Scope, NotFound>(new Scope_GetByName
        {
            Name = name
        }, cancellationToken);

        return response switch
        {
            (_, Scope scope) => Map(scope),
            (_, NotFound) => null,
            _ => throw new InvalidOperationException()
        };
    }

    public override async IAsyncEnumerable<ScopeModel> FindByNamesAsync(ImmutableArray<string> names,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Scope_GetByNames>();

        var response = await client.GetResponse<Multiple<Scope>>(new Scope_GetByNames
        {
            Names = names.ToList()
        }, cancellationToken);

        foreach (var scope in response.Message.Items)
        {
            yield return Map(scope);
        }
    }

    public override async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateRequestClient<Scope_Count>();

        var response = await client.GetResponse<Count>(new Scope_Count(), cancellationToken);

        return response.Message.Value;
    }

    public override ValueTask<ScopeModel> InstantiateAsync(CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public override ValueTask CreateAsync(ScopeModel scope, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public override ValueTask DeleteAsync(ScopeModel scope, CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public override ValueTask UpdateAsync(ScopeModel scope, CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public override IAsyncEnumerable<ScopeModel> FindByResourceAsync(string resource,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}