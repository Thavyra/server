namespace Thavyra.Rest.Features.Users.Roles.Get;

public class Response
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
}