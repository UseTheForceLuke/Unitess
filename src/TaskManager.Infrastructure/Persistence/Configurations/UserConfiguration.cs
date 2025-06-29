using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Users;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(100);
        builder.Property(u => u.Role).IsRequired();
    }
}