namespace Thavyra.Contracts.Login.Providers;

public record ProviderLinked
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
}