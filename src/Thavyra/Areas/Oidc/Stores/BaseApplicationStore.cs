using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using Thavyra.Oidc.Models.Internal;

namespace Thavyra.Oidc.Stores;

public abstract class BaseApplicationStore : IOpenIddictApplicationStore<ApplicationModel>
{
    // Repository style methods to be implemented by derived types.

    public abstract ValueTask<ApplicationModel> InstantiateAsync(CancellationToken cancellationToken);

    public abstract ValueTask CreateAsync(ApplicationModel application, CancellationToken cancellationToken);

    public abstract ValueTask UpdateAsync(ApplicationModel application, CancellationToken cancellationToken);

    public abstract ValueTask DeleteAsync(ApplicationModel application, CancellationToken cancellationToken);
    
    public abstract ValueTask<long> CountAsync(CancellationToken cancellationToken);
    
    public abstract IAsyncEnumerable<ApplicationModel> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken);
    
    public abstract ValueTask<ApplicationModel?> FindByIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract ValueTask<ApplicationModel?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<ApplicationModel> FindByPostLogoutRedirectUriAsync(string uri,
        CancellationToken cancellationToken);

    public abstract IAsyncEnumerable<ApplicationModel> FindByRedirectUriAsync(string uri, CancellationToken cancellationToken);
    
    // Manage redirects, permissions and requirements
    
    public abstract ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(ApplicationModel application,
        CancellationToken cancellationToken);

    public virtual ValueTask SetRedirectUrisAsync(ApplicationModel application, ImmutableArray<string> uris,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();


    public abstract ValueTask<ImmutableArray<string>> GetPermissionsAsync(ApplicationModel application,
        CancellationToken cancellationToken);
    
    public ValueTask SetPermissionsAsync(ApplicationModel application, ImmutableArray<string> permissions,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    
    public abstract ValueTask<ImmutableArray<string>> GetRequirementsAsync(ApplicationModel application,
        CancellationToken cancellationToken);

    public ValueTask SetRequirementsAsync(ApplicationModel application, ImmutableArray<string> requirements,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    
    public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public virtual ValueTask SetPostLogoutRedirectUrisAsync(ApplicationModel application, ImmutableArray<string> uris,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    // EF methods, unsupported by default
    
    public virtual ValueTask<long> CountAsync<TResult>(
        Func<IQueryable<ApplicationModel>, IQueryable<TResult>> query,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public virtual ValueTask<TResult?> GetAsync<TState, TResult>(
        Func<IQueryable<ApplicationModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
        Func<IQueryable<ApplicationModel>, TState, IQueryable<TResult>> query, TState state,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    // Supported getters/setters
    
    public ValueTask<string?> GetApplicationTypeAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => new(application.ApplicationType);

    public ValueTask SetApplicationTypeAsync(ApplicationModel application, string? type,
        CancellationToken cancellationToken)
    {
        application.ApplicationType = type;
        return new ValueTask();
    }
    

    public ValueTask<string?> GetClientIdAsync(ApplicationModel application, CancellationToken cancellationToken) 
        => new(application.ClientId);

    public ValueTask SetClientIdAsync(ApplicationModel application, string? identifier,
        CancellationToken cancellationToken)
    {
        application.ClientId = identifier;
        return new ValueTask();
    }
    
    
    public ValueTask<string?> GetClientTypeAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => new(application.ClientType);

    public ValueTask SetClientTypeAsync(ApplicationModel application, string? type,
        CancellationToken cancellationToken)
    {
        application.ClientType = type;
        return new ValueTask();
    }
    
    
    public ValueTask<string?> GetConsentTypeAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => new(application.ConsentType);

    public ValueTask SetConsentTypeAsync(ApplicationModel application, string? type,
        CancellationToken cancellationToken)
    {
        application.ConsentType = type;
        return new ValueTask();
    }
    

    public ValueTask<string?> GetDisplayNameAsync(ApplicationModel application, CancellationToken cancellationToken)
        => new(application.DisplayName);

    public ValueTask SetDisplayNameAsync(ApplicationModel application, string? name,
        CancellationToken cancellationToken)
    {
        application.DisplayName = name;
        return new ValueTask();
    }
    
    
    public ValueTask<string?> GetIdAsync(ApplicationModel application, CancellationToken cancellationToken)
        => new(application.Id.ToString());

    
    // Unsupported getters/setters
    
    public ValueTask<string?> GetClientSecretAsync(ApplicationModel application, CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public ValueTask SetClientSecretAsync(ApplicationModel application, string? secret,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();


    public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => new(ImmutableDictionary.Create<CultureInfo, string>());
    
    public ValueTask SetDisplayNamesAsync(ApplicationModel application, ImmutableDictionary<CultureInfo, string> names,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();


    public ValueTask<JsonWebKeySet?> GetJsonWebKeySetAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public ValueTask SetJsonWebKeySetAsync(ApplicationModel application, JsonWebKeySet? set,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    
    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
    public ValueTask SetPropertiesAsync(ApplicationModel application,
        ImmutableDictionary<string, JsonElement> properties,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();


    public ValueTask<ImmutableDictionary<string, string>> GetSettingsAsync(ApplicationModel application,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public ValueTask SetSettingsAsync(ApplicationModel application, ImmutableDictionary<string, string> settings,
        CancellationToken cancellationToken)
        => throw new NotSupportedException();
    
}
