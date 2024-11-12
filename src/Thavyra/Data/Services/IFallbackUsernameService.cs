namespace Thavyra.Data.Services;

public interface IFallbackUsernameService
{
    Task<string> GenerateFallbackUsernameAsync();
}