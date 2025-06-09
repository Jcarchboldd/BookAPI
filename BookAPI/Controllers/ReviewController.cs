using Microsoft.AspNetCore.Authorization;

namespace BookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    /// <summary>
    /// Retrieves all reviews for a specific book.
    /// </summary>
    /// <param name="bookId">The ID of the book.</param>
    /// <returns>A list of reviews for the book.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/review/bookReviews/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <response code="200">Returns the list of reviews</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("bookReviews/{bookId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> GetBookReviewsAsync(Guid bookId)
    {
        var bookReviews = await reviewService.GetAllReviewsAsync(bookId);

        return Ok(bookReviews);
    }

    /// <summary>
    /// Retrieves a review by its unique ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review.</param>
    /// <returns>The requested review.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/review/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <response code="200">Returns the requested review</response>
    /// <response code="404">Review not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{reviewId:guid}", Name = nameof(GetReviewByIdAsync))]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> GetReviewByIdAsync(Guid reviewId)
    {
        var result = await reviewService.GetReviewByIdAsync(reviewId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new review.
    /// </summary>
    /// <param name="request">The review information to create.</param>
    /// <returns>The ID of the newly created review.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/review
    ///     {
    ///         "content": "Great book!",
    ///         "rating": 5,
    ///         "bookId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    /// <response code="201">Review created successfully.</response>
    /// <response code="400">Validation error or bad request.</response>
    /// <response code="500">Internal server error.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> CreateReviewAsync(CreateReviewRequest request)
    {
        var reviewId = await reviewService.CreateReviewAsync(request);
        return CreatedAtRoute(
            nameof(GetReviewByIdAsync),
            new { reviewId },
            new { id = reviewId });
    }

    /// <summary>
    /// Updates an existing review.
    /// </summary>
    /// <param name="request">The updated review information.</param>
    /// <returns>No content if the update was successful.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/review
    ///     {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "content": "Updated review",
    ///         "rating": 4,
    ///         "bookId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// </remarks>
    /// <response code="204">Review updated successfully</response>
    /// <response code="400">Validation error or bad request</response>
    /// <response code="404">Review not found</response>
    /// <response code="500">Internal server error</response>
    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
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