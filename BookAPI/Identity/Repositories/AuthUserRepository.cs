using BookAPI.Identity.Models;

namespace BookAPI.Identity.Repositories;

public class AuthUserRepository(BookDbContext bookDbContext, IDateTimeProvider clock) : IAuthUserRepository
{
    public async Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await bookDbContext.AuthUsers.SingleOrDefaultAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(AuthUser user, CancellationToken cancellationToken)
    {
        var now = clock.UtcNow;
        user.CreatedDateTime = now;
        user.UpdatedDateTime = now;
        await bookDbContext.AddAsync(user, cancellationToken);
        
    }
}