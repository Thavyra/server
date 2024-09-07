namespace Thavyra.Contracts.Application;

/// <summary>
/// Returns <see cref="Multiple{T}"/> of <see cref="Redirect"/>
/// </summary>
public record Redirect_GetByApplication
{
    public required Guid ApplicationId { get; init; }
}