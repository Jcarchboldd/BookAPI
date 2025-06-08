using Microsoft.AspNetCore.Authorization;

namespace BookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("bookReviews/{bookId:guid}")]
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReviewAsync(CreateReviewRequest request)
    {
        var reviewId = await reviewService.CreateReviewAsync(request);
        return CreatedAtRoute(
            nameof(GetReviewByIdAsync),
            new { reviewId },
            new { id = reviewId });
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateReviewAsync(UpdateReviewRequest request)
    {
        await reviewService.UpdateReviewAsync(request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a review by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the review to delete.</param>
    /// <returns>No content if the review was deleted successfully.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/review/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <response code="204">Review deleted successfully</response>
    /// <response code="404">Review not found</response>
    /// <response code="500">Internal server error</response>
    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> DeleteReviewAsync(Guid id)
    {
        await reviewService.DeleteReviewAsync(id);
        return NoContent();
    }
}