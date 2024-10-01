using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thavyra.Contracts.Permission;
using Thavyra.Data.Models;

namespace Thavyra.Data.Contexts;

public class ThavyraDbContext : DbContext
{
    public ThavyraDbContext() { }
    
    public ThavyraDbContext(DbContextOptions<ThavyraDbContext> options) : base(options)
    {
        
    }

    public DbSet<UserDto> Users { get; set; }
    public DbSet<RoleDto> Roles { get; set; }
    
    public DbSet<PasswordLoginDto> Passwords { get; set; }
    public DbSet<DiscordLoginDto> DiscordLogins { get; set; }
    public DbSet<GitHubLoginDto> GitHubLogins { get; set; }
    
    public DbSet<ApplicationDto> Applications { get; set; }
    public DbSet<RedirectDto> Redirects { get; set; }

    public DbSet<PermissionDto> Permissions { get; set; }
    public DbSet<ScopeDto> Scopes { get; set; }
    public DbSet<AuthorizationDto> Authorizations { get; set; }
    public DbSet<TokenDto> Tokens { get; set; }

    
    
    public DbSet<TransactionDto> Transactions { get; set; }

    public DbSet<ObjectiveDto> Objectives { get; set; }
    public DbSet<ScoreDto> Scores { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ConfigureRoles(builder.Entity<RoleDto>());
        
        ConfigureUsers(builder.Entity<UserDto>());
        
        ConfigureDiscordLogins(builder.Entity<DiscordLoginDto>());
        ConfigureGitHubLogins(builder.Entity<GitHubLoginDto>());

        ConfigurePermissions(builder.Entity<PermissionDto>());

        ConfigureApplications(builder.Entity<ApplicationDto>());

        ConfigureRedirects(builder.Entity<RedirectDto>());

        ConfigureScopes(builder.Entity<ScopeDto>());

        ConfigureTokens(builder.Entity<TokenDto>());

        ConfigureObjectives(builder.Entity<ObjectiveDto>());
    }
    
    protected EntityTypeBuilder<RoleDto> ConfigureRoles(EntityTypeBuilder<RoleDto> entity)
    {
        entity
            .HasIndex(e => e.Name)
            .IsUnique();

        return entity;
    }
    
    protected EntityTypeBuilder<UserDto> ConfigureUsers(EntityTypeBuilder<UserDto> entity)
    {
        entity
            .HasIndex(e => e.Username)
            .IsUnique();

        entity
            .HasMany(e => e.Roles)
            .WithMany(e => e.Users)
            .UsingEntity<UserRoleDto>(
                l => l.HasOne<RoleDto>().WithMany().HasForeignKey(e => e.RoleId),
                r => r.HasOne<UserDto>().WithMany().HasForeignKey(e => e.UserId));

        return entity;
    }
    
    protected EntityTypeBuilder<DiscordLoginDto> ConfigureDiscordLogins(EntityTypeBuilder<DiscordLoginDto> entity)
    {
        entity
            .HasIndex(e => e.DiscordId)
            .IsUnique();

        return entity;
    }
    
    protected EntityTypeBuilder<GitHubLoginDto> ConfigureGitHubLogins(EntityTypeBuilder<GitHubLoginDto> entity)
    {
        entity
            .HasIndex(e => e.GitHubId)
            .IsUnique();

        return entity;
    }
    
    protected EntityTypeBuilder<PermissionDto> ConfigurePermissions(EntityTypeBuilder<PermissionDto> entity)
    {
        entity
            .HasIndex(e => e.Name)
            .IsUnique();

        return entity;
    }
    
    protected EntityTypeBuilder<ApplicationDto> ConfigureApplications(EntityTypeBuilder<ApplicationDto> entity)
    {
        entity
            .HasIndex(e => e.ClientId)
            .IsUnique();

        entity
            .HasMany(e => e.Permissions)
            .WithMany(e => e.Applications)
            .UsingEntity<ApplicationPermissionDto>(
                l => l.HasOne<PermissionDto>().WithMany().HasForeignKey(e => e.PermissionId),
                r => r.HasOne<ApplicationDto>().WithMany().HasForeignKey(e => e.ApplicationId));

        return entity;
    }
    
    protected EntityTypeBuilder<RedirectDto> ConfigureRedirects(EntityTypeBuilder<RedirectDto> entity)
    {
        entity
            .HasIndex(e => e.Uri);

        return entity;
    }
    
    protected EntityTypeBuilder<ScopeDto> ConfigureScopes(EntityTypeBuilder<ScopeDto> entity)
    {
        entity
            .HasIndex(e => e.Name)
            .IsUnique();

        entity
            .HasMany(e => e.Authorizations)
            .WithMany(e => e.Scopes)
            .UsingEntity<AuthorizationScopeDto>(
                l => l.HasOne<AuthorizationDto>().WithMany().HasForeignKey(e => e.AuthorizationId),
                r => r.HasOne<ScopeDto>().WithMany().HasForeignKey(e => e.ScopeId));

        return entity;
    }
    
    protected EntityTypeBuilder<TokenDto> ConfigureTokens(EntityTypeBuilder<TokenDto> entity)
    {
        entity
            .HasIndex(e => e.ReferenceId)
            .IsUnique();

        return entity;
    }
    
    protected EntityTypeBuilder<ObjectiveDto> ConfigureObjectives(EntityTypeBuilder<ObjectiveDto> entity)
    {
        entity
            .HasIndex(e => e.Name);

        return entity;
    }
}