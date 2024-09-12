namespace Thavyra.Rest.Security.Resource;

/// <summary>
/// Represents a resource to be created.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Create<T>
{
    private Create(Contracts.User.User user)
    {
        User = user;
    }

    public Contracts.User.User User { get; }

    public static Create<T> For(Contracts.User.User user) => new(user);
}