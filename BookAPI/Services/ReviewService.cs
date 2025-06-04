namespace BookAPI.Services;

public class ReviewService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : IReviewService
{
    public async Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync(Guid bookId)
    {
        var result = await unitOfWork.ReviewRepository.GetBookReviewsAsync(bookId);
        
        return result.Adapt<IEnumerable<ReviewResponse>>();
    }

    public async Task<ReviewResponse?> GetReviewByIdAsync(Guid id)
    {
        var result = await unitOfWork.ReviewRepository.GetReviewByIdAsync(id);
        
        return result.Adapt<ReviewResponse>();
    }

    public async Task<Guid> CreateReviewAsync(CreateReviewRequest request)
    {
        await ValidationHandler.ValidateAsync(request, serviceProvider);
        
        var review = request.Adapt<Review>();
        await unitOfWork.ReviewRepository.CreateReviewAsync(review);
        await unitOfWork.SaveAsync();
        return review.Id;
    }

    public async Task UpdateReviewAsync(UpdateReviewRequest request)
    {
        await ValidationHandler.ValidateAsync(request, serviceProvider);
        
        var review = request.Adapt<Review>();
        await unitOfWork.ReviewRepository.UpdateReviewAsync(review);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteReviewAsync(Guid id)
    {
        await unitOfWork.ReviewRepository.DeleteReviewAsync(id);
        await unitOfWork.SaveAsync();
    }
}