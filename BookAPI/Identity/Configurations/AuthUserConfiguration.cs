using BookAPI.Identity.Models;

namespace BookAPI.Identity.Configurations;

public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("AuthUsers");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.CreatedDateTime)
            .IsRequired();

        builder.Property(u => u.UpdatedDateTime)
            .IsRequired();

    }
}