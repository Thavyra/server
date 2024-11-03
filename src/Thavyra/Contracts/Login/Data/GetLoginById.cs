namespace Thavyra.Contracts.Login.Data;

public record GetLoginById
{
    public required Guid LoginId { get; init; }   
}