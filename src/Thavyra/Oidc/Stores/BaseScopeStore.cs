using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using OpenIddict.Abstractions;
using Thavyra.Oidc.Models;

namespace Thavyra.Oidc.Stores;

public abstract class BaseScopeStore : IOpenIddictScopeStore<ScopeModel>
{
    // Repository methods
    
    public abstract ValueTask<ScopeModel> InstantiateAsync(CancellationToken cancellationToken);
    
    public abstract ValueTask CreateAsync(ScopeModel scope, CancellationToken cancellationToken);

    public abstract ValueTask UpdateAsync(ScopeModel scope, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(ScopeModel scope, CancellationToken cancellationToken);

    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);
    
    public abstract IAsyncEnumerable<ScopeModel> ListAsync(int? count, int? offset, CancellationToken cancellationToken);

    public abstract ValueTask<ScopeModel?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<ScopeModel?> FindByNameAsync(string name, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<ScopeModel> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<ScopeModel> FindByResourceAsync(string resource, CancellationToken cancellationToken);

    // EF
    
    public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<ScopeModel>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public virtual ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<ScopeModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<ScopeModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    // Supported getters/setters
    
    public ValueTask<string?> GetDescriptionAsync(ScopeModel scope, CancellationToken cancellationToken)
        => new(scope.Description);
    
    public ValueTask SetDescriptionAsync(ScopeModel scope, string? description, CancellationToken cancellationToken)
    {
        scope.Description = description;
        return new ValueTask();
    }


    public ValueTask<string?> GetDisplayNameAsync(ScopeModel scope, CancellationToken cancellationToken)
        => new(scope.DisplayName);
    
    public ValueTask SetDisplayNameAsync(ScopeModel scope, string? name, CancellationToken cancellationToken)
    {
        scope.DisplayName = name;
        return new ValueTask();
    }


    public ValueTask<string?> GetIdAsync(ScopeModel scope, CancellationToken cancellationToken)
        => new(scope.Id.ToString());


    public ValueTask<string?> GetNameAsync(ScopeModel scope, CancellationToken cancellationToken)
        => new(scope.Name);
    
    public ValueTask SetNameAsync(ScopeModel scope, string? name, CancellationToken cancellationToken)
    {
        scope.Name = name;
        return new ValueTask();
    }
    
    // Unsupported getters/setters

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(ScopeModel scope,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetDescriptionsAsync(ScopeModel scope, ImmutableDictionary<CultureInfo, string> descriptions,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    
    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(ScopeModel scope,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetDisplayNamesAsync(ScopeModel scope, ImmutableDictionary<CultureInfo, string> names,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();


    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(ScopeModel scope,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetPropertiesAsync(ScopeModel scope, ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();


    public ValueTask<ImmutableArray<string>> GetResourcesAsync(ScopeModel scope,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetResourcesAsync(ScopeModel scope, ImmutableArray<string> resources,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
}
