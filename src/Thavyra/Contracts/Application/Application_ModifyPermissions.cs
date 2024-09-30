namespace Thavyra.Contracts.Application;

public record Application_ModifyPermissions
{
    public required Guid ApplicationId { get; init; }
    public required List<Guid> Grant { get; init; }
    public required List<Guid> Deny { get; init; }
}