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
        modelBuilder.Entity<ApplicationDto>().ToTable("applications");
        modelBuilder.Entity<RedirectDto>().ToTable("redirects");
        
        modelBuilder.Entity<ScopeDto>().ToTable("scopes");
        modelBuilder.Entity<AuthorizationDto>().ToTable("authorizations");
        modelBuilder.Entity<TokenDto>().ToTable("tokens");
        
        modelBuilder.Entity<UserDto>().ToTable("users");
        modelBuilder.Entity<PasswordLoginDto>().ToTable("passwords");
    }
}