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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScopeDto>()
            .HasMany(e => e.Authorizations)
            .WithMany(e => e.Scopes)
            .UsingEntity<AuthorizationScopeDto>(
                l => l.HasOne<AuthorizationDto>().WithMany().HasForeignKey(e => e.AuthorizationId),
                r => r.HasOne<ScopeDto>().WithMany().HasForeignKey(e => e.ScopeId));
    }
}