using BookAPI.Identity.Repositories;

namespace BookAPI.Infrastructure.Data;

public interface IUnitOfWork
{
    IBookRepository BookRepository { get; }
    IReviewRepository ReviewRepository { get; }
    IAuthUserRepository AuthUserRepository { get;}
    Task SaveAsync();
}