using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Determines whether the claims principal contains the given scope, or any of its parents.
    /// </summary>
    /// <para>
    /// Scopes are formed of n parts separated by '.' e.g. account.profile.read
    /// </para>
    /// <para>
    /// Possessing a parent scope allows use of its children e.g. account.profile automatically allows account.profile.read and account.profile.edit
    /// </para>
    /// <param name="user"></param>
    /// <param name="scope"></param>
    /// <returns>true if the principal contains the given scopes</returns>
    public static bool HasRelativeScope(this ClaimsPrincipal user, string scope)
    {
        string[] parts = scope.Split('.');

        for (int i = 1; i <= parts.Length; i++)
        {
            string combined = string.Join('.', parts[..i]);

            if (user.HasScope(combined))
            {
                return true;
            }
        }

        return false;
    }

    public static Guid GetSubject(this ClaimsPrincipal user)
    {
        if (user.GetClaim(OpenIddictConstants.Claims.Subject) is not { } claim)
        {
            throw new InvalidOperationException("Principal does not have subject claim.");
        }

        if (!Guid.TryParse(claim, out var subject))
        {
            throw new InvalidOperationException("Subject claim is not a Guid.");
        }

        return subject;
    }

    public static Guid GetClient(this ClaimsPrincipal user)
    {
        if (user.GetClaim(Constants.Claims.ApplicationId) is not { } claim)
        {
            throw new InvalidOperationException("Principal does not have client claim.");
        }

        if (!Guid.TryParse(claim, out var client))
        {
            throw new InvalidOperationException("Client claim is not a Guid.");
        }

        return client;
    }
    
    public static bool HasSubject(this ClaimsPrincipal user, Guid subject)
    {
        return user.HasClaim(OpenIddictConstants.Claims.Subject, subject.ToString());
    }

    public static bool HasClient(this ClaimsPrincipal user, Guid client)
    {
        return user.HasClaim(Constants.Claims.ApplicationId, client.ToString());
    }
}