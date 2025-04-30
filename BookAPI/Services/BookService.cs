namespace BookAPI.Services;

public class BookService(IUnitOfWork unitOfWork, IValidator<CreateBookRequest> createValidator) : IBookService
{
    public async Task<IEnumerable<BookResponse>> GetAllBooksAsync()
    {
        var result = await unitOfWork.BookRepository.GetAllBooksAsync();
        return result.Adapt<IEnumerable<BookResponse>>();
    }

    public async Task<BookResponse?> GetBookByIdAsync(Guid id)
    {
        var result = await unitOfWork.BookRepository.GetBookByIdAsync(id);
        return result.Adapt<BookResponse>();
    }

    public async Task<Guid> CreateBookAsync(CreateBookRequest book)
    {
        var validationResult = await createValidator.ValidateAsync(book);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var newBook = book.Adapt<Book>();
        await unitOfWork.BookRepository.CreateBookAsync(newBook);
        await unitOfWork.SaveAsync();
        
        return newBook.Id;
    }

    public async Task UpdateBookAsync(UpdateBookRequest book)
    {
        var updatedBook = book.Adapt<Book>();
        await unitOfWork.BookRepository.UpdateBookAsync(updatedBook);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        await unitOfWork.BookRepository.DeleteBookAsync(id);
        await unitOfWork.SaveAsync();
    }
}