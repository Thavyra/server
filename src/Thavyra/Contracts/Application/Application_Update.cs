namespace Thavyra.Contracts.Application;

public record Application_Update
{
    public required Guid Id { get; init; }
    
    public Change<string> Name { get; set; }
    public Change<string?> Description { get; set; }
}