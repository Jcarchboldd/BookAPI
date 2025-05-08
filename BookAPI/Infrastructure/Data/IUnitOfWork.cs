using BookAPI.Identity.Repositories;

namespace BookAPI.Infrastructure.Data;

public interface IUnitOfWork : IDisposable
{
    IBookRepository BookRepository { get; }
    
    IAuthUserRepository AuthUserRepository { get;}
    Task SaveAsync();
}