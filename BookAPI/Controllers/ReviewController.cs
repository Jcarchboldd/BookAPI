namespace BookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("book/{bookId:guid}")]
    public async Task<IActionResult> GetBookReviewsAsync(Guid bookId)
    {
        var bookReviews = await reviewService.GetAllReviewsAsync(bookId);
        
        return Ok(bookReviews);
    }
}