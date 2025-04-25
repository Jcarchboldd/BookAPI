using BookAPI.Infrastructure.Repositories;

namespace BookAPI.Infrastructure.Data;

public interface IUnitOfWork : IDisposable
{
    IBookRepository BookRepository { get; }
    Task SaveAsync();
}