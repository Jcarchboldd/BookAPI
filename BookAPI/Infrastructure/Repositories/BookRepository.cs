namespace BookAPI.Infrastructure.Repositories;

public class BookRepository(BookDbContext context) : IBookRepository
{
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await context.Books
            .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await context.Books
            .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(b => b.Id == id);
            
    }

    public async Task CreateBookAsync(Book book)
    {
        await context.Books.AddAsync(book);
    }

    public async Task UpdateBookAsync(Book book)
    {
        var existingBook = await context.Books.FindAsync(book.Id);
        if (existingBook is null)
        {
            throw new NotFoundException(nameof(Book), book.Id);
        }

        context.Entry(existingBook).CurrentValues.SetValues(book);
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var existingBook = await context.Books.FindAsync(id);
        if (existingBook is null)
        {
            throw new NotFoundException(nameof(Book), id);
        }
        
        context.Books.Remove(existingBook);
    }
}