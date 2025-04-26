using BookAPI.Contracts.Books;

namespace BookAPI.Services;

public class BookService : IBookService
{
    public Task<IEnumerable<BookResponse>> GetAllBooksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BookResponse?> GetBookByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task CreateBookAsync(CreateBookRequest book)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBookAsync(UpdateBookRequest book)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBookAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}