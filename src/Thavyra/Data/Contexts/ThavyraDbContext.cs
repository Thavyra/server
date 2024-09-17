using Microsoft.EntityFrameworkCore;
using Thavyra.Data.Models;

namespace Thavyra.Data.Contexts;

public class ThavyraDbContext : DbContext
{
    public ThavyraDbContext(DbContextOptions<ThavyraDbContext> options) : base(options)
    {
        
    }

    public DbSet<ApplicationDto> Applications { get; set; }
    public DbSet<RedirectDto> Redirects { get; set; }

    public DbSet<ScopeDto> Scopes { get; set; }
    public DbSet<AuthorizationDto> Authorizations { get; set; }
    public DbSet<TokenDto> Tokens { get; set; }

    public DbSet<UserDto> Users { get; set; }
    public DbSet<PasswordLoginDto> Passwords { get; set; }
    public DbSet<DiscordLoginDto> DiscordLogins { get; set; }
    public DbSet<GitHubLoginDto> GitHubLogins { get; set; }
    
    public DbSet<TransactionDto> Transactions { get; set; }

    public DbSet<ObjectiveDto> Objectives { get; set; }
    public DbSet<ScoreDto> Scores { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserDto>()
            .HasIndex(e => e.Username)
            .IsUnique();
        
        builder.Entity<DiscordLoginDto>()
            .HasIndex(e => e.DiscordId)
            .IsUnique();

        builder.Entity<GitHubLoginDto>()
            .HasIndex(e => e.GitHubId)
            .IsUnique();

        builder.Entity<ApplicationDto>()
            .HasIndex(e => e.ClientId)
            .IsUnique();

        builder.Entity<RedirectDto>()
            .HasIndex(e => e.Uri);

        var scope = builder.Entity<ScopeDto>();
        
        scope
            .HasIndex(e => e.Name)
            .IsUnique();
        
        scope
            .HasMany(e => e.Authorizations)
            .WithMany(e => e.Scopes)
            .UsingEntity<AuthorizationScopeDto>(
                l => l.HasOne<AuthorizationDto>().WithMany().HasForeignKey(e => e.AuthorizationId),
                r => r.HasOne<ScopeDto>().WithMany().HasForeignKey(e => e.ScopeId));

        builder.Entity<TokenDto>()
            .HasIndex(e => e.ReferenceId)
            .IsUnique();
    }
}