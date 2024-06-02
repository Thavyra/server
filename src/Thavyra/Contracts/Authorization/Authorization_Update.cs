namespace Thavyra.Contracts.Authorization;

public record Authorization_Update
{
    public required Guid Id { get; init; }
    
    public Change<string?> Type { get; init; }
    public Change<string?> Status { get; init; }
}