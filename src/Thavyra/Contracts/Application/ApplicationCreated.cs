namespace Thavyra.Contracts.Application;

public record ApplicationCreated
{
    public required Guid Id { get; init; }
    public required Application Application { get; init; }
    public required string? ClientSecret { get; init; }
}