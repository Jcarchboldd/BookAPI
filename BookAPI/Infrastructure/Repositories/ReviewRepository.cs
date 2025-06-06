namespace BookAPI.Infrastructure.Repositories;

public class ReviewRepository(BookDbContext bookDbContext) : IReviewRepository
{
    public async Task<IEnumerable<Review>> GetBookReviewsAsync(Guid bookId)
    {
        return await bookDbContext.Reviews
            .Include(i => i.Book)
            .Include(i => i.User)
            .Where(w => w.Book.Id.Equals(bookId))
            .ToListAsync();
    }

    public Task<Review?> GetReviewByIdAsync(Guid id)
    {
        return bookDbContext.Reviews
            .Include(i => i.Book)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateReviewAsync(Review review)
    {
        await bookDbContext.Reviews.AddAsync(review);
    }

    public async Task UpdateReviewAsync(Review review)
    {
        var existingReview = await bookDbContext.Reviews.FindAsync(review.Id);
        if (existingReview == null)
        {
            throw new NotFoundException(nameof(review), review.Id); 
        }
        
        bookDbContext.Entry(existingReview).CurrentValues.SetValues(review);
    }

    public async Task DeleteReviewAsync(Guid id)
    {
        var reviewToDelete = await bookDbContext.Reviews.FindAsync(id);
        if (reviewToDelete == null)
        {
            throw new NotFoundException(nameof(reviewToDelete), id);
        }
        bookDbContext.Reviews.Remove(reviewToDelete);
    }
}