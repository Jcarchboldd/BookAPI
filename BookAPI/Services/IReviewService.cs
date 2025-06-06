namespace BookAPI.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync(Guid bookId);
    Task<ReviewResponse?> GetReviewByIdAsync(Guid id);
    Task<Guid> CreateReviewAsync(CreateReviewRequest request);
    Task UpdateReviewAsync(UpdateReviewRequest request);
    Task DeleteReviewAsync(Guid id);
}