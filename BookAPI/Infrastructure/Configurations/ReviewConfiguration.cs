namespace BookAPI.Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.BookId)
            .IsRequired();
        
        builder.Property(r => r.UserId)
            .IsRequired();
        
        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}