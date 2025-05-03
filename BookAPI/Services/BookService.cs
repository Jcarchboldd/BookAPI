namespace BookAPI.Services;

public class BookService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : IBookService
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
        await ValidateAsync(book);
        
        var newBook = book.Adapt<Book>();
        await unitOfWork.BookRepository.CreateBookAsync(newBook);
        await unitOfWork.SaveAsync();
        
        return newBook.Id;
    }

    public async Task UpdateBookAsync(UpdateBookRequest book)
    {
        await ValidateAsync(book);
        
        var updatedBook = book.Adapt<Book>();
        await unitOfWork.BookRepository.UpdateBookAsync(updatedBook);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        await unitOfWork.BookRepository.DeleteBookAsync(id);
        await unitOfWork.SaveAsync();
    }
    
    private async Task ValidateAsync<T>(T request)
    {
        var validator = serviceProvider.GetService<IValidator<T>>();
        if (validator is null)
        {
            throw new InternalServerException(
                $"Missing validator for type {typeof(T).Name}",
                $"Validator of type IValidator<{typeof(T).Name}> was not registered in the DI container.");
        }

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}