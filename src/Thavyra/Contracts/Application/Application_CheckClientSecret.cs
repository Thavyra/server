namespace Thavyra.Contracts.Application;

/// <summary>
/// Checks the specified application's client secret.
/// </summary>
/// <returns><see cref="Correct"/>, <see cref="Incorrect"/></returns>
public record Application_CheckClientSecret
{
    public required Guid ApplicationId { get; init; }
    public required string Secret { get; init; }
}