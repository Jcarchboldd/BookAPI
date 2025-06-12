using BookAPI.Identity.Models;

namespace BookAPI.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.IsVerified)
            .IsRequired();

        builder.Property(u => u.AuthUserId)
            .IsRequired(false);

        builder.HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.AuthUser)
            .WithOne(au => au.User)
            .HasForeignKey<User>(u => u.AuthUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.AuthUserId).IsUnique();
    }
}