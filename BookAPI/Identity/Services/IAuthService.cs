using BookAPI.Identity.Contracts;
using BookAPI.Identity.Models;

namespace BookAPI.Identity.Services;

public interface IAuthService
{
    Task<AuthenticationResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    
    Task<AuthUser?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);
}