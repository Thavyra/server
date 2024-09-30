namespace Thavyra.Contracts.Permission;

public record Permission_GetByNames
{
    public required List<string> Names { get; init; }   
}