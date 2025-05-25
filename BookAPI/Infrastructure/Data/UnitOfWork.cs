using BookAPI.Identity.Repositories;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookAPI.Infrastructure.Data;

public class UnitOfWork(BookDbContext context, IDateTimeProvider dateTimeProvider) : IUnitOfWork
{
    private IBookRepository? _bookRepository;
    private IAuthUserRepository? _authUserRepository;
    private IReviewRepository? _reviewRepository;
    
    public IBookRepository BookRepository => _bookRepository ??= new BookRepository(context);
    public IReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(context);
    public IAuthUserRepository AuthUserRepository => _authUserRepository ??= new AuthUserRepository(context, dateTimeProvider);

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
    
    // TODO: Check if the Dispose method is required 
    public void Dispose() => context.Dispose();
}