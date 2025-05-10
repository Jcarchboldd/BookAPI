using BookAPI.Identity.Contracts;
using BookAPI.Identity.Models;

namespace BookAPI.Identity.Services;

public interface IAuthService
{
    Task<Guid> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    
    Task<AuthenticationResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken);
}