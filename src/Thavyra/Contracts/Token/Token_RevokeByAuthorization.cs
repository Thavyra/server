namespace Thavyra.Contracts.Token;

public record Token_RevokeByAuthorization
{
    public required string AuthorizationId { get; init; }
}