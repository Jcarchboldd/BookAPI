using BookAPI.Identity.Models;

namespace BookAPI.Identity.Repositories;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(AuthUser user, CancellationToken cancellationToken);
}