namespace BookAPI.Infrastructure.Models;

public class Review
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}