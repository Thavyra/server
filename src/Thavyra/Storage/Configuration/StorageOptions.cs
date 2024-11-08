namespace Thavyra.Storage.Configuration;

public class StorageOptions
{
    public AvatarOptions Avatars { get; set; } = new();
}

public class AvatarOptions
{
    public string Bucket { get; set; } = null!;
    public UserAvatarOptions Users { get; set; } = new();
    public ApplicationAvatarOptions Applications { get; set; } = new();
}

public class UserAvatarOptions
{
    public string Prefix { get; set; } = null!;
}

public class ApplicationAvatarOptions
{
    public string Prefix { get; set; } = null!;
}