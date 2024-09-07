namespace Thavyra.Contracts.Token;

/// <summary>
/// Removes the tokens that are marked as invalid or whose attached authorization is no longer valid. Only tokens created before the specified threshold are removed.
/// </summary>
/// <returns><see cref="Count"/></returns>
public record Token_Prune
{
    public required DateTime Threshold { get; init; }
}