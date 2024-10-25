using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Migrations;

public class MigrationDbContext : ThavyraDbContext
{
    private readonly IConfiguration _configuration;
    private readonly EntityOptions _entityOptions;

    public MigrationDbContext(IConfiguration configuration, IOptions<EntityOptions> entityOptions)
    {
        _configuration = configuration;
        _entityOptions = entityOptions.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (_configuration["UseConnection"])
        {
            case "Postgres":
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Postgres"));
                break;
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ConfigureRoles(builder.Entity<RoleDto>())
            .HasData(_entityOptions.Roles);

        ConfigureUsers(builder.Entity<UserDto>())
            .HasData(_entityOptions.Users);

        ConfigureLogins(builder.Entity<LoginDto>());

        ConfigurePermissions(builder.Entity<PermissionDto>())
            .HasData(_entityOptions.Permissions);

        ConfigureApplications(builder.Entity<ApplicationDto>())
            .HasData(_entityOptions.Applications);

        builder.Entity<ApplicationPermissionDto>()
            .HasData(_entityOptions.ApplicationPermissions);

        ConfigureRedirects(builder.Entity<RedirectDto>());

        ConfigureScopes(builder.Entity<ScopeDto>())
            .HasData(_entityOptions.Scopes);

        ConfigureTokens(builder.Entity<TokenDto>());

        ConfigureObjectives(builder.Entity<ObjectiveDto>());
    }
}