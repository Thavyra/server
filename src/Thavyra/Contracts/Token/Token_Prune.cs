namespace Thavyra.Contracts.Token;

public record Token_Prune
{
    public required DateTime Threshold { get; init; }
}