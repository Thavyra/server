namespace Thavyra.Contracts.Application;

/// <summary>
/// Resets the application's client secret, or creates one if not already set.
/// </summary>
public record Application_ResetClientSecret
{
    public required Guid ApplicationId { get; init; }   
}