namespace Thavyra.Contracts.Scoreboard;

public record Score_GetById
{
    public required Guid Id { get; init; }
}