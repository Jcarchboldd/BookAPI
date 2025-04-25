using BookAPI.Infrastructure.Data;

namespace BookAPI.Infrastructure.Repositories;

public class BookRepository(BookDbContext context) : IBookRepository
{
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await context.Books
            .Include(b => b.Reviews)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await context.Books
            .Include(b => b.Reviews)
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
            //TODO: Replace with custom NotFoundException
            throw new Exception();
        }

        context.Entry(existingBook).CurrentValues.SetValues(book);
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var existingBook = await context.Books.FindAsync(id);
        if (existingBook is null)
        {
            //TODO: Replace with custom NotFoundException
            throw new Exception();
        }
        
        context.Books.Remove(existingBook);
    }
}