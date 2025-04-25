namespace BookAPI.Infrastructure.Data;

public class UnitOfWork(BookDbContext context) : IUnitOfWork
{
    private IBookRepository? _bookRepository;
    
    public IBookRepository BookRepository => _bookRepository ??= new BookRepository(context);

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
    
    public void Dispose() => context.Dispose();
}