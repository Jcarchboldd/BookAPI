namespace BookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBooksAsync()
    {
        var result = await bookService.GetAllBooksAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBookByIdAsync(Guid id)
    {
        var result = await bookService.GetBookByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookAsync(CreateBookRequest book)
    {
        var bookId = await bookService.CreateBookAsync(book);
        return CreatedAtAction(
            nameof(GetBookByIdAsync), 
            controllerName: null, 
            routeValues: new { id = bookId }, 
            value: book
        );

    }

    [HttpPut]
    public async Task<IActionResult> UpdateBookAsync(UpdateBookRequest book)
    {
        await bookService.UpdateBookAsync(book);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBookAsync(Guid id)
    {
        await bookService.DeleteBookAsync(id);
        return NoContent();
    }
    
}