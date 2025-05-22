using BookAPI.Identity.Configurations;
using BookAPI.Identity.Contracts;
using BookAPI.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Identity.Services;

public class AuthService(
    IUnitOfWork uow,
    IPasswordHasher<AuthUser> hasher,
    IJwtTokenGenerator jwt) : IAuthService
{
    public async Task<Guid> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        // Check for duplicate email
        if (await uow.AuthUserRepository.GetUserByEmailAsync(request.Email, cancellationToken) != null)
            throw new BadRequestException("Email is already registered.");

        // Create & hash
        var user = new AuthUser
        {
            Id        = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName  = request.LastName,
            Email     = request.Email
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        // Persist
        await uow.AuthUserRepository.AddAsync(user, cancellationToken);
        await uow.SaveAsync();

        return user.Id;

    }

    public async Task<AuthenticationResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await uow.AuthUserRepository.GetUserByEmailAsync(request.Email, cancellationToken)
                   ?? throw new UnauthorizedException("Invalid credentials.");
        
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result != PasswordVerificationResult.Success)
            throw new UnauthorizedException("Invalid credentials.");
        
        var token = jwt.GenerateToken(user);
        
        return new AuthenticationResponse(
            user.Id.ToString(),
            user.FirstName,
            user.LastName,
            user.Email,
            token);
    }
}