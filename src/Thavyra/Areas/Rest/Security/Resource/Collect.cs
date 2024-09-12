namespace Thavyra.Rest.Security.Resource;

/// <summary>
/// Represents a collection of resources belonging to a user.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Collect<T>
{
    private Collect(Contracts.User.User user)
    {
        User = user;
    }

    public Contracts.User.User User { get; }

    public static Collect<T> For(Contracts.User.User user) => new(user);
}