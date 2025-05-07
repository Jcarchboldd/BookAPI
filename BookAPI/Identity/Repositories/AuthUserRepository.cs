using BookAPI.Identity.Models;

namespace BookAPI.Identity.Repositories;

public class AuthUserRepository(BookDbContext bookDbContext) : IAuthUserRepository
{
    public async Task<AuthUser?> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        return await bookDbContext.AuthUsers.SingleOrDefaultAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }

    public async Task Add(AuthUser user, CancellationToken cancellationToken)
    {
        await bookDbContext.AddAsync(user, cancellationToken);
        
    }
}