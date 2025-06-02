namespace BookAPI.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(BookDbContext context)
    {
        if (context.Books.Any()) return;

        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), UserName = "jane_doe", IsVerified = true },
            new() { Id = Guid.NewGuid(), UserName = "john_smith", IsVerified = false }
        };

        var books = new List<Book>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "1984",
                Author = "George Orwell"
            }
        };

        var reviews = new List<Review>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = "An amazing read with deep meaning.",
                Rating = 5,
                BookId = books[0].Id,
                UserId = users[0].Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Thought-provoking and chilling.",
                Rating = 4,
                BookId = books[1].Id,
                UserId = users[1].Id
            }
        };

        context.Users.AddRange(users);
        context.Books.AddRange(books);
        context.Reviews.AddRange(reviews);
        await context.SaveChangesAsync();
    }
}