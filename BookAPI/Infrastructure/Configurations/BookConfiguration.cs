namespace BookAPI.Infrastructure.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasMany(b => b.Reviews)
            .WithOne(b => b.Book)
            .HasForeignKey(b => b.BookId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}