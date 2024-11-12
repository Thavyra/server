namespace Thavyra.Contracts.Authorization;

/// <summary>
/// Deletes all authorizations marked as invalid, and ad-hoc authorizations with no corresponding token. Only authorizations created before the specified threshold are removed. Returns the number of removed records.
/// </summary>
/// <returns><see cref="Count"/></returns>
public record Authorization_Prune
{
    public required DateTime Threshold { get; init; }
}