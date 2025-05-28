namespace BookAPI.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync();
    Task<ReviewResponse?> GetReviewAsync(Guid id);
    Task<Guid> CreateReviewAsync(CreateReviewRequest request);
    Task UpdateReviewAsync(UpdateReviewRequest request);
    Task DeleteReviewAsync(Guid id);
}