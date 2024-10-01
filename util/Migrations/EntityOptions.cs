using Thavyra.Data.Models;

namespace Migrations;

public class EntityOptions
{
    public List<ScopeDto> Scopes { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
    public List<RoleDto> Roles { get; set; } = [];
    public List<UserDto> Users { get; set; } = [];
    public List<ApplicationDto> Applications { get; set; } = [];
    public List<ApplicationPermissionDto> ApplicationPermissions { get; set; } = [];
}