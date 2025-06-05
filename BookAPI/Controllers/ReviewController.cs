namespace BookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("bookReviews{bookId:guid}")]
    public async Task<IActionResult> GetBookReviewsAsync(Guid bookId)
    {
        var bookReviews = await reviewService.GetAllReviewsAsync(bookId);
        
        return Ok(bookReviews);
    }

    [HttpGet("{reviewId:guid}", Name = nameof(GetReviewByIdAsync))]
    public async Task<IActionResult> GetReviewByIdAsync(Guid reviewId)
    {
        var result = await reviewService.GetReviewByIdAsync(reviewId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}