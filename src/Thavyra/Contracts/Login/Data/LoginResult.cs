namespace Thavyra.Contracts.Login.Data;

public record LoginResult
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }

    public required string Type { get; init; }
    
    public string? ProviderAccountId { get; init; }
    public string? ProviderUsername { get; init; }
    public string? ProviderAvatarUrl { get; init; }
    
    public required DateTime UpdatedAt { get; init; }
    public required DateTime UsedAt { get; set; }
    public required DateTime CreatedAt { get; init; }
}