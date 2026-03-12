using BookRepositoryApi.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRepositoryApi.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
              .ValueGeneratedOnAdd();
        builder.Property(u => u.Username).HasMaxLength(64).IsRequired();
        builder.Property(u => u.NormalizedUsername).HasMaxLength(64).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(32).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasIndex(u => u.NormalizedUsername).IsUnique();
    }
}