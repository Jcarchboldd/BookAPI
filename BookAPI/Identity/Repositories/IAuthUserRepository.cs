using BookAPI.Identity.Models;

namespace BookAPI.Identity.Repositories;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetUserByEmail(string email, CancellationToken cancellationToken);
    Task Add(AuthUser user, CancellationToken cancellationToken);
}