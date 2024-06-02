namespace Thavyra.Contracts.Token;

public record Token_GetByUser
{
    public required string UserId { get; init; }
}