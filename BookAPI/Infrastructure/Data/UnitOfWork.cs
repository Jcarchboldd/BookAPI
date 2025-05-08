using BookAPI.Identity.Repositories;

namespace BookAPI.Infrastructure.Data;

public class UnitOfWork(BookDbContext context) : IUnitOfWork
{
    private IBookRepository? _bookRepository;
    private IAuthUserRepository? _authUserRepository;
    
    public IBookRepository BookRepository => _bookRepository ??= new BookRepository(context);
    public IAuthUserRepository AuthUserRepository => _authUserRepository ??= new AuthUserRepository(context);

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
    
    public void Dispose() => context.Dispose();
}