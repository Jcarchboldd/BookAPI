namespace BookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController(IBookService bookService) : ControllerBase
{
    /// <summary>
    /// Retrieves all books.
    /// </summary>
    /// <returns>A list of all books.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/book
    /// </remarks>
    /// <response code="200">Returns the list of books</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookResponse>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> GetBooksAsync()
    {
        var result = await bookService.GetAllBooksAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a book by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The book with the specified ID.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <response code="200">Returns the requested book</response>
    /// <response code="404">Book not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> GetBookByIdAsync(Guid id)
    {
        var result = await bookService.GetBookByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="book">The book information to create.</param>
    /// <returns>The ID of the newly created book.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/book
    ///     {
    ///         "title": "The Great Gatsby",
    ///         "author": "F. Scott Fitzgerald"
    ///     }
    /// </remarks>
    /// <response code="201">Book created successfully.</response>
    /// <response code="400">Validation error or bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> CreateBookAsync(CreateBookRequest book)
    {
        var bookId = await bookService.CreateBookAsync(book);
        var actionName = nameof(GetBookByIdAsync);
        return CreatedAtAction(
            actionName,
            new { id = bookId },
            new { id = bookId }
        );

    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="book">The updated book information.</param>
    /// <returns>No content if the update was successful.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/book
    ///     {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "title": "Updated Title",
    ///         "author": "Updated Author"
    ///     }
    /// </remarks>
    /// <response code="204">Book updated successfully</response>
    /// <response code="400">Validation error or bad request</response>
    /// <response code="404">Book not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> UpdateBookAsync(UpdateBookRequest book)
    {
        await bookService.UpdateBookAsync(book);
        return NoContent();
    }

    /// <summary>
    /// Deletes a book by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the book to delete.</param>
    /// <returns>No content if the book was deleted successfully.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <response code="204">Book deleted successfully</response>
    /// <response code="404">Book not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<IActionResult> DeleteBookAsync(Guid id)
    {
        await bookService.DeleteBookAsync(id);
        return NoContent();
    }
    
}