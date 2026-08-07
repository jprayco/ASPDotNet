using ManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementSystem.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
        entity.HasIndex(u => u.Email).IsUnique();
        entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        entity.Property(u => u.IsActive).IsRequired().HasDefaultValue(false);

        entity.HasMany(u => u.Roles)
              .WithMany(r => r.Users)
              .UsingEntity(j => j.ToTable("UserRoles"));
    }
}
