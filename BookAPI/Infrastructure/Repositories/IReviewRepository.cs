namespace BookAPI.Infrastructure.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllReviewsAsync();
    Task<Review?> GetReviewByIdAsync(Guid id);
    Task CreateReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(Guid id);

}