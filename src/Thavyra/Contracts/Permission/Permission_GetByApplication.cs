namespace Thavyra.Contracts.Permission;

public record Permission_GetByApplication
{
    public required Guid ApplicationId { get; init; }
}