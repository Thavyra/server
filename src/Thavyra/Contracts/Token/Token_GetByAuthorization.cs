namespace Thavyra.Contracts.Token;

public record Token_GetByAuthorization
{
    public required string AuthorizationId { get; init; }
}