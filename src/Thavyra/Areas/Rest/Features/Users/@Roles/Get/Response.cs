namespace Thavyra.Rest.Features.Users.Roles.Get;

public class Response
{
    /// <summary>
    /// id of the role.
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Name of the role.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Display name of the role.
    /// </summary>
    public required string DisplayName { get; set; }
}