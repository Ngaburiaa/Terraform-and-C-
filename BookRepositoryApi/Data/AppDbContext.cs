using BookRepositoryApi.Data.Configurations;
using BookRepositoryApi.Models;
using BookRepositoryApi.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace BookRepositoryApi.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookConfiguration());
        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
    }
}

