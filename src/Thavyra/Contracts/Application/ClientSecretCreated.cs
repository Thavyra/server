namespace Thavyra.Contracts.Application;

public record ClientSecretCreated
{
    public required Guid ApplicationId { get; init; }
    public required string ClientSecret { get; init; }
}