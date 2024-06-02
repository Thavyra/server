namespace Thavyra.Contracts.Token;

public record Token_GetByApplication
{
    public required string ApplicationId { get; init; }
}