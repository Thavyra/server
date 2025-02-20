namespace Thavyra.Rest.Features.Users.Roles.Delete;

public class Request : UserRequest
{
    /// <summary>
    /// id of the role.
    /// </summary>
    public Guid Id { get; set; }
}