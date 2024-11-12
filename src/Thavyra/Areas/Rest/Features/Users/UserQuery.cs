namespace Thavyra.Rest.Features.Users;

public abstract class UserQuery
{
    public static bool TryParse(string? value, out UserQuery? result)
    {
        result = value switch
        {
            _ when Guid.TryParse(value, out var g) => new UserIdQuery(g),
            "@me" => new SelfQuery(),
            ['@', .. { } username] => new UsernameQuery(username),
            _ => null
        };

        return result is not null;
    }
}

public class UserIdQuery : UserQuery
{
    public UserIdQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}

public class SelfQuery : UserQuery;

public class UsernameQuery : UserQuery
{
    public UsernameQuery(string username)
    {
        Username = username;
    }

    public string Username { get; }
}
