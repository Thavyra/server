namespace Thavyra.Contracts.Application;

/// <summary>
/// Finds all applications associated with the specified redirect. Returns <see cref="NotFound"/> if the redirect does not exist.
/// </summary>
/// <returns><see cref="Multiple{T}"/> of <see cref="Application"/>, <see cref="NotFound"/></returns>
public record Application_GetByRedirect
{
    public required string Uri { get; init; }
}