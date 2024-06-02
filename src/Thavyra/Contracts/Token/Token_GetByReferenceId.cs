namespace Thavyra.Contracts.Token;

public record Token_GetByReferenceId
{
    public required string ReferenceId { get; init; }
}