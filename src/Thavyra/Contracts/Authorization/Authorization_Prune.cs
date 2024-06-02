namespace Thavyra.Contracts.Authorization;

public record Authorization_Prune
{
    public required DateTime Threshold { get; init; }
}