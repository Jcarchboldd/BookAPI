using BookAPI.Contracts.Books;

namespace BookAPI.Services;

public interface IBookService
{
    Task<IEnumerable<BookResponse>> GetAllBooksAsync();
    Task<BookResponse?> GetBookByIdAsync(Guid id);
    Task<Guid> CreateBookAsync(CreateBookRequest book);
    Task UpdateBookAsync(UpdateBookRequest book);
    Task DeleteBookAsync(Guid id);
}