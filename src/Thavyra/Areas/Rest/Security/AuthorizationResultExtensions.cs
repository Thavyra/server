using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security;

public static class AuthorizationResultExtensions
{
    public static bool Failed(this AuthorizationResult result) => !result.Succeeded;
}