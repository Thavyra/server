namespace Thavyra.Contracts.Application;

public record Application_GetByClientId
{
    public required string ClientId { get; init; }
}