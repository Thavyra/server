namespace Thavyra.Contracts.Application;

public record Application_ValidateClientSecret
{
    public required Guid ApplicationId { get; init; }
    public required string Secret { get; init; }
}